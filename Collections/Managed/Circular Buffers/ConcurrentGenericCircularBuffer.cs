using System;
using System.Threading;

namespace HereticalSolutions.Collections.Managed
{
    //Courtesy of https://github.com/joaoportela/CircularBuffer-CSharp/blob/master/CircularBuffer/CircularBuffer.cs
    public class ConcurrentGenericCircularBuffer<TValue>
    {
        private TValue[] contents;

        private BufferElementDescriptor[] descriptors;
        

        private volatile int producerStart;

        private volatile int producerEnd;

        private volatile int consumerStart;

        private volatile int consumerEnd;

        private volatile uint freeKey;


        public int ProducerStart { get { return Interlocked.Read(ref producerStart); } }

        public int ProducerEnd { get { return Interlocked.Read(ref producerEnd); } }

        public int ConsumerStart { get { return Interlocked.Read(ref consumerStart); } }

        public int ConsumerEnd { get { return Interlocked.Read(ref consumerEnd); } }

        public uint FreeKey { get { return Interlocked.Read(ref freeKey); } }


        public ConcurrentGenericCircularBuffer(
            TValue[] contents,
            BufferElementDescriptor[] descriptors)
        {
            this.contents = contents;

            this.descriptors = descriptors;
            
            producerStart = 0;
            
            producerEnd = 0;

            consumerStart = 0;

            consumerEnd = 0;

            freeKey = 0;
        }


        public void RequestProduceNonAlloc(ref ProducerTicket result)
        {
            //Allocate unique key for producer
            int key = Interlocked.Increment(ref freeKey);

            //Create the new descriptor pre-emptively
            //Fill out the descriptor copy with desired values
            BufferElementDescriptor newDescriptor = new BufferElementDescriptor
            {
                Key = key,

                State = EBufferElementState.ALLOCATED_FOR_PRODUCER
            };

            //Prepare to SPIN
            SpinWait spin = new SpinWait();

            while (true)
            {
                //Read the current producer queue end index
                int currentProducerEnd = Interlocked.Read(ref producerEnd);                

                //Read the descriptor of the current producer queue end
                var currentDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[currentProducerEnd],
                    default(BufferElementDescriptor),
                    default(BufferElementDescriptor));

                //If it's not vacant then produce failure
                if (currentDescriptor.State != EBufferElementState.VACANT)
                {
                    result.RequestSuccess = false;

                    result.Key = -1;

                    result.Index = -1;

                    return;
                }

                //Try to write new descriptor values
                var updatedDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[currentProducerEnd],
                    newDescriptor,
                    currentDescriptor);

                //Proceed only if succeeded
                if (updatedDescriptor == currentDescriptor)
                {
                    //Increment and wrap the producer queue end index
                    int newProducerEnd = IncrementAndWrap(currentProducerEnd);

                    //Write new index
                    int updatedProducerEnd = Interlocked.CompareExchange(
                        ref producerEnd,
                        newProducerEnd,
                        currentProducerEnd);
                    
                    //Success
                    if (currentProducerEnd == updatedProducerEnd)
                    {
                        result.RequestSuccess = true;

                        result.Key = key;

                        result.Index = currentProducerEnd;

                        return;
                    }

                    //Try again
                    spin.SpinOnce();
                }

                //Looks like the descriptor was overwritten. Spin and repeat
                spin.SpinOnce();
            }

            //This should not happen
            result.RequestSuccess = false;

            result.Key = -1;

            result.Index = -1;
        }

        public bool Produce(ProducerTicket ticket, TValue newValue)
        {
            //Skip invalid tickets
            if (!ticket.RequestSuccess)
                return false;

            //Create the new descriptor pre-emptively
            //Fill out the descriptor copy with desired values
            BufferElementDescriptor newDescriptor = new BufferElementDescriptor
            {
                Key = -1,

                State = EBufferElementState.FILLED
            };

            //Prepare to SPIN
            SpinWait spin = new SpinWait();

            while (true)
            {
                //Read the descriptor at the ticket index
                var currentDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[ticket.Index],
                    default(BufferElementDescriptor),
                    default(BufferElementDescriptor));

                //Skip invalid tickets
                if (currentDescriptor.State != EBufferElementState.ALLOCATED_FOR_PRODUCER)
                    return false;

                if (currentDescriptor.Key != ticket.Key)
                    return false;

                //Produce
                Interlocked.Exchange<TValue>(
                    ref contents[ticket.Index],
                    newValue);

                //Try to write new descriptor values
                var updatedDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[currentProducerEnd],
                    newDescriptor,
                    currentDescriptor);

                //Proceed only if succeeded
                if (updatedDescriptor == currentDescriptor)
                {
                    while (true)
                    {
                        //Read the current producer queue start index
                        int currentProducerStart = Interlocked.Read(ref producerStart);

                        int newProducerStart = currentProducerStart;

                        //Iterate from start index to ticket index to check whether all elements are filled
                        while (newProducerStart != ticket.Index)
                        {
                            var currentIndexDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                                ref descriptors[newProducerStart],
                                default(BufferElementDescriptor),
                                default(BufferElementDescriptor));

                            //if (currentIndexDescriptor.State == EBufferElementState.ALLOCATED_FOR_PRODUCER)
                            //    break;

                            if (currentIndexDescriptor.State != EBufferElementState.FILLED)
                            {
                                break;
                            }

                            newProducerStart = IncrementAndWrap(newProducerStart);
                        }

                        //We have omitted checking the descriptor at ticket index on purpose - we've already changed it to desired value
                        //If all the tickets before current are filled
                        if (newProducerStart == ticket.Index)
                            newProducerStart++;

                        //If nothing changed in terms of producer start value, just return
                        if (newProducerStart == currentProducerStart)
                            return true;

                        //Write new index
                        int updatedProducerStart = Interlocked.CompareExchange(
                            ref producerStart,
                            newProducerStart,
                            currentProducerStart);

                        //Success
                        if (currentProducerStart == updatedProducerStart)
                        {
                            return true;
                        }

                        //Try again
                        spin.SpinOnce();
                    }

                    return true;
                }

                //Looks like the descriptor was overwritten. Spin and repeat
                spin.SpinOnce();
            }

            //This should not happen
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int IncrementAndWrap(int index)
        {
            return (++index) % contents.Length;
        }


        /*        
        private ref TValue this[int index]
        {
            get
            {
                int actualIndex = InternalIndex(index);
                
                return ref contents[actualIndex];
            }
        }
        
        public ref TValue PushBack()
        {
            ref TValue result = ref this[end];
            
            end = Increment(end);
            
            if (!HasFreeSpace)
            {
                start = Increment(end);
            }
            else
            {
                ++Count;
            }
            
            return ref result;
        }
        
        public ref TValue PopFront()
        {
            ThrowIfEmpty("Cannot take elements from an empty buffer.");
            
            ref TValue result = ref this[Start];
            
            Start = Increment(Start);
            
            --Count;
            
            return ref result;
        }
        
        public void Clear()
        {
            Start = 0;
            
            End = 0;
            
            Count = 0;
            
            //Optional
            //Buffer.Clear(memoryChunk.Memory, 0, memoryChunk.Size);
        }
        
        private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Increment(int index)
        {
            if (++index == ElementCapacity)
            {
                index = 0;
            }
            
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Decrement(int index)
        {
            if (index == 0)
            {
                index = ElementCapacity;
            }
            
            index--;
            
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int InternalIndex(int index)
        {
            return Start + (
                (index < (ElementCapacity - Start))
                    ? index
                    : index - ElementCapacity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int InternalIndex(uint index)
        {
            var result = Start + (
                (index < (ElementCapacity - Start))
                    ? index
                    : index - ElementCapacity);
            
            return (int)result;
        }
        */
    }   
}