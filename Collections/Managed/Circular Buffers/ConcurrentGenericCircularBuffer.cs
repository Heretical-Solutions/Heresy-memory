using System;
using System.Threading;

namespace HereticalSolutions.Collections.Managed
{
    public class ConcurrentGenericCircularBuffer<TValue>
    {
        private volatile TValue[] contents;

        private volatile BufferElementDescriptor[] descriptors;
        

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
            #region Prepare the values to be written to a descriptor array

            //Allocate unique key for producer
            int key = Interlocked.Increment(ref freeKey);

            //Create the new descriptor pre-emptively
            //Fill out the descriptor copy with desired values
            BufferElementDescriptor newDescriptor = new BufferElementDescriptor
            {
                Key = key,

                State = EBufferElementState.ALLOCATED_FOR_PRODUCER
            };

            #endregion

            //Prepare to SPIN
            SpinWait spin = new SpinWait();

            while (true)
            {
                //Read the current producer queue end index
                int currentProducerEnd = Interlocked.Read(ref producerEnd);

                //Increment and wrap the producer queue end index
                int newProducerEnd = IncrementAndWrap(currentProducerEnd);

                #region Ouroboros prevention mechanism

                //Read the current consumer queue start and end indexes
                int currentConsumerStart = Interlocked.Read(ref consumerStart);

                int currentConsumerEnd = Interlocked.Read(ref consumerEnd);

                //Unwrap if necessary
                if (currentConsumerEnd < currentConsumerStart)
                    currentConsumerEnd += contents.Length;

                //If new producer end is within consumer queue then produce failure
                if (newProducerEnd >= currentConsumerStart && newProducerEnd <= currentConsumerEnd)
                {
                    result.RequestSuccess = false;

                    result.Key = -1;

                    result.Index = -1;

                    return;
                }

                #endregion

                //Read the descriptor of the current producer queue end
                var currentDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[currentProducerEnd],
                    default(BufferElementDescriptor),
                    default(BufferElementDescriptor));

                #region Overwrite prevention mechanism

                //If it's not vacant then produce failure
                if (currentDescriptor.State != EBufferElementState.VACANT)
                {
                    result.RequestSuccess = false;

                    result.Key = -1;

                    result.Index = -1;

                    return;
                }

                #endregion

                #region Allocate a descriptor at the producer end queue and increment producer end index

                //Try to write new descriptor values
                var updatedDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[currentProducerEnd],
                    newDescriptor,
                    currentDescriptor);

                //Proceed only if succeeded
                if (updatedDescriptor == currentDescriptor)
                {
                    #region Update ticket

                    result.RequestSuccess = true;

                    result.Key = key;

                    result.Index = currentProducerEnd;

                    #endregion

                    #region Increment producer end index

                    while (true)
                    {
                        //Write new index
                        int updatedProducerEnd = Interlocked.CompareExchange(
                            ref producerEnd,
                            newProducerEnd,
                            currentProducerEnd);
                        
                        //Success
                        if (currentProducerEnd == updatedProducerEnd)
                        {
                            return;
                        }

                        #region Faulty producer end index prevention mechanism

                        //Read the current producer queue start index
                        int currentProducerStart = Interlocked.Read(ref producerStart);

                        int updatedProducerEndUnwrapped = updatedProducerEnd;

                        //Unwrap if necessary
                        if (updatedProducerEnd < currentProducerStart)
                            updatedProducerEndUnwrapped += contents.Length;

                        //If updated producer end is greater than new producer end then leave it as be and call it a day
                        if (updatedProducerEndUnwrapped > newProducerEnd)
                        {
                            return;
                        }

                        #endregion

                        currentProducerEnd = updatedProducerEnd;

                        //Try again
                        spin.SpinOnce();
                    }

                    #endregion

                    return;
                }

                //Looks like the descriptor was overwritten by another thread. Spin and repeat
                spin.SpinOnce();

                #endregion
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

            #region Prepare the values to be written to a descriptor array

            //Create the new descriptor pre-emptively
            //Fill out the descriptor copy with desired values
            BufferElementDescriptor newDescriptor = new BufferElementDescriptor
            {
                Key = -1,

                State = EBufferElementState.FILLED
            };

            #endregion

            //Prepare to SPIN
            SpinWait spin = new SpinWait();

            while (true)
            {
                //Read the descriptor at the ticket index
                var currentDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[ticket.Index],
                    default(BufferElementDescriptor),
                    default(BufferElementDescriptor));

                #region Validate ticket

                //Skip invalid tickets
                if (currentDescriptor.State != EBufferElementState.ALLOCATED_FOR_PRODUCER)
                    return false;

                if (currentDescriptor.Key != ticket.Key)
                    return false;

                #endregion

                //Produce
                Interlocked.Exchange<TValue>(
                    ref contents[ticket.Index],
                    newValue);

                #region Update the descriptor at the ticket index and increment producer start index

                //Try to write new descriptor values
                var updatedDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[ticket.Index],
                    newDescriptor,
                    currentDescriptor);

                //Proceed only if succeeded
                if (updatedDescriptor == currentDescriptor)
                {
                    #region Increment producer start index

                    while (true)
                    {
                        //Read the current producer queue start index
                        int currentProducerStart = Interlocked.Read(ref producerStart);
                        
                        #region Faulty producer start index prevention mechanism

                        //If it's already greater than the ticket index then just return
                        if (currentProducerStart > ticket.Index)
                            return true;

                        int currentProducerEnd = Interlocked.Read(ref producerEnd);

                        if (currentProducerStart < ticket.Index 
                            && currentProducerEnd < ticket.Index
                            && currentProducerEnd >= currentProducerStart)
                            return true;

                        #endregion

                        int newProducerStart = -1;

                        if (currentProducerStart == ticket.Index)
                        {
                            newProducerStart = IncrementAndWrap(ticket.Index);
                        }
                        else
                        {
                            #region Check whether all descriptors starting with current producer queue start up to ticket index are filled

                            //Iterate from current start index to ticket index to check whether all elements are filled
                            for (int i = currentProducerStart;
                                 i != ticket.Index;
                                 i = IncrementAndWrap(i))
                            {
                                var currentIndexDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                                    ref descriptors[i],
                                    default(BufferElementDescriptor),
                                    default(BufferElementDescriptor));

                                if (currentIndexDescriptor.State != EBufferElementState.FILLED)
                                {
                                    break;
                                }
                                else
                                    newProducerStart = i;
                            }

                            //We have omitted checking the descriptor at ticket index on purpose - we've already changed it to desired value
                            //If all the tickets before current are filled
                            if (newProducerStart == ticket.Index)
                                newProducerStart = IncrementAndWrap(ticket.Index);

                            #endregion
                        }

                        #region Early return if updating producer queue start index is useless

                        if (newProducerStart == -1)
                            return true;

                        //If nothing changed in terms of producer start value, just return
                        if (newProducerStart == currentProducerStart)
                            return true;

                        #endregion

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

                    #endregion
                }

                #endregion

                //Looks like the descriptor was overwritten. Spin and repeat
                spin.SpinOnce();
            }

            //This should not happen
            return false;
        }

        public void RequestConsumeNonAlloc(ref ConsumerTicket result)
        {
            #region Prepare the values to be written to a descriptor array

            //Allocate unique key for consumer
            int key = Interlocked.Increment(ref freeKey);

            //Create the new descriptor pre-emptively
            //Fill out the descriptor copy with desired values
            BufferElementDescriptor newDescriptor = new BufferElementDescriptor
            {
                Key = key,

                State = EBufferElementState.ALLOCATED_FOR_CONSUMER
            };

            #endregion

            //Prepare to SPIN
            SpinWait spin = new SpinWait();

            while (true)
            {
                //Read the current consumer queue end index
                int currentConsumerEnd = Interlocked.Read(ref consumerEnd);

                //Increment and wrap the consumer queue end index
                int newConsumerEnd = IncrementAndWrap(currentConsumerEnd);

                #region Overconsuming prevention mechanism

                //Read the current producer queue start and end indexes
                int currentProducerStart = Interlocked.Read(ref producerStart);

                int currentProducerEnd = Interlocked.Read(ref producerEnd);

                //Unwrap if necessary
                if (currentProducerEnd < currentProducerStart)
                    currentProducerEnd += contents.Length;

                //If new consumer end is within producer queue then consume failure
                if (newConsumerEnd >= currentProducerStart && newConsumerEnd <= currentProducerEnd)
                {
                    result.RequestSuccess = false;

                    result.Key = -1;

                    result.Index = -1;

                    return;
                }

                #endregion

                //Read the descriptor of the current consumer queue end
                var currentDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[currentConsumerEnd],
                    default(BufferElementDescriptor),
                    default(BufferElementDescriptor));

                #region Overwrite prevention mechanism

                //If it's not vacant then produce failure
                if (currentDescriptor.State != EBufferElementState.FILLED)
                {
                    result.RequestSuccess = false;

                    result.Key = -1;

                    result.Index = -1;

                    return;
                }

                #endregion

                #region Allocate a descriptor at the consumer end queue and increment consumer end index

                //Try to write new descriptor values
                var updatedDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[currentConsumerEnd],
                    newDescriptor,
                    currentDescriptor);

                //Proceed only if succeeded
                if (updatedDescriptor == currentDescriptor)
                {
                    #region Update ticket

                    result.RequestSuccess = true;

                    result.Key = key;

                    result.Index = currentConsumerEnd;

                    #endregion

                    #region Increment consumer end index

                    while (true)
                    {
                        //Write new index
                        int updatedConsumerEnd = Interlocked.CompareExchange(
                            ref consumerEnd,
                            newConsumerEnd,
                            currentConsumerEnd);
                        
                        //Success
                        if (currentConsumerEnd == updatedConsumerEnd)
                        {
                            return;
                        }

                        #region Faulty consumer end index prevention mechanism

                        //Read the current consumer queue start index
                        int currentConsumerStart = Interlocked.Read(ref consumerStart);

                        int updatedConsumerEndUnwrapped = updatedConsumerEnd;

                        //Unwrap if necessary
                        if (updatedConsumerEnd < currentConsumerStart)
                            updatedConsumerEndUnwrapped += contents.Length;

                        //If updated consumer end is greater than new consumer end then leave it as be and call it a day
                        if (updatedConsumerEndUnwrapped > newConsumerEnd)
                        {
                            return;
                        }

                        #endregion

                        currentConsumerEnd = updatedConsumerEnd;

                        //Try again
                        spin.SpinOnce();
                    }

                    #endregion

                    return;
                }

                //Looks like the descriptor was overwritten by another thread. Spin and repeat
                spin.SpinOnce();

                #endregion
            }

            //This should not happen
            result.RequestSuccess = false;

            result.Key = -1;

            result.Index = -1;
        }

        public bool Consume(ConsumerTicket ticket, out TValue newValue)
        {
            //Skip invalid tickets
            if (!ticket.RequestSuccess)
                return false;

            #region Prepare the values to be written to a descriptor array

            //Create the new descriptor pre-emptively
            //Fill out the descriptor copy with desired values
            BufferElementDescriptor newDescriptor = new BufferElementDescriptor
            {
                Key = -1,

                State = EBufferElementState.VACANT
            };

            #endregion

            //Prepare to SPIN
            SpinWait spin = new SpinWait();

            while (true)
            {
                //Read the descriptor at the ticket index
                var currentDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[ticket.Index],
                    default(BufferElementDescriptor),
                    default(BufferElementDescriptor));

                #region Validate ticket

                //Skip invalid tickets
                if (currentDescriptor.State != EBufferElementState.ALLOCATED_FOR_CONSUMER)
                    return false;

                if (currentDescriptor.Key != ticket.Key)
                    return false;

                #endregion

                //Consume
                result = Interlocked.CompareExchange<TValue>(
                    ref contents[ticket.Index],
                    default(TValue),
                    default(TValue));

                #region Update the descriptor at the ticket index and increment consumer start index

                //Try to write new descriptor values
                var updatedDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                    ref descriptors[ticket.Index],
                    newDescriptor,
                    currentDescriptor);

                //Proceed only if succeeded
                if (updatedDescriptor == currentDescriptor)
                {
                    #region Increment consumer start index

                    while (true)
                    {
                        //Read the current consumer queue start index
                        int currentConsumerStart = Interlocked.Read(ref consumerStart);
                        
                        #region Faulty consumer start index prevention mechanism

                        //If it's already greater than the ticket index then just return
                        if (currentConsumerStart > ticket.Index)
                            return true;

                        int currentConsumerEnd = Interlocked.Read(ref consumerEnd);

                        if (currentConsumerStart < ticket.Index 
                            && currentConsumerEnd < ticket.Index
                            && currentConsumerEnd >= currentConsumerStart)
                            return true;

                        #endregion

                        int newConsumerStart = -1;

                        if (currentConsumerStart == ticket.Index)
                        {
                            newConsumerStart = IncrementAndWrap(ticket.Index);
                        }
                        else
                        {
                            #region Check whether all descriptors starting with current consumer queue start up to ticket index are vacant

                            //Iterate from current start index to ticket index to check whether all elements are vacant
                            for (int i = currentConsumerStart;
                                 i != ticket.Index;
                                 i = IncrementAndWrap(i))
                            {
                                var currentIndexDescriptor = Interlocked.CompareExchange<BufferElementDescriptor>(
                                    ref descriptors[i],
                                    default(BufferElementDescriptor),
                                    default(BufferElementDescriptor));

                                if (currentIndexDescriptor.State != EBufferElementState.VACANT)
                                {
                                    break;
                                }
                                else
                                    newConsumerStart = i;
                            }

                            //We have omitted checking the descriptor at ticket index on purpose - we've already changed it to desired value
                            //If all the tickets before current are filled
                            if (newConsumerStart == ticket.Index)
                                newConsumerStart = IncrementAndWrap(ticket.Index);

                            #endregion
                        }

                        #region Early return if updating consumer queue start index is useless

                        if (newConsumerStart == -1)
                            return true;

                        //If nothing changed in terms of consumer start value, just return
                        if (newConsumerStart == currentConsumerStart)
                            return true;

                        #endregion

                        //Write new index
                        int updatedConsumerStart = Interlocked.CompareExchange(
                            ref consumerStart,
                            newConsumerStart,
                            currentConsumerStart);

                        //Success
                        if (currentConsumerStart == updatedConsumerStart)
                        {
                            return true;
                        }

                        //Try again
                        spin.SpinOnce();
                    }

                    return true;

                    #endregion
                }

                #endregion

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
    }   
}