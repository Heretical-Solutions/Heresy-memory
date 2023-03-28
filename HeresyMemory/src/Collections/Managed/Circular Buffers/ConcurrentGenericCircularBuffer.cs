using System.Threading;
using System.Runtime.CompilerServices;

namespace HereticalSolutions.Collections.Managed
{
    /// <summary>
    /// An MPMC circular buffer
    /// </summary>
    public class ConcurrentGenericCircularBuffer<TValue> where TValue : class
    {
        #region Consts
        
        //Enum values stored in consts to quicken the state comparisons
        //We still need enum itself for serialization purposes
        
        /// <summary>
        /// Cached vacant state
        /// </summary>
        private const int STATE_VACANT = (int)EBufferElementState.VACANT;
        
        /// <summary>
        /// Cached allocated for producer state
        /// </summary>
        private const int STATE_ALLOCATED_FOR_PRODUCER = (int)EBufferElementState.ALLOCATED_FOR_PRODUCER;
        
        /// <summary>
        /// Cached filled state
        /// </summary>
        private const int STATE_FILLED = (int)EBufferElementState.FILLED;
        
        /// <summary>
        /// Cached allocated for consumer state
        /// </summary>
        private const int STATE_ALLOCATED_FOR_CONSUMER = (int)EBufferElementState.ALLOCATED_FOR_CONSUMER;
        
        #endregion
        
        /// <summary>
        /// The contents array
        /// </summary>
        private volatile TValue[] contents;

        /// <summary>
        /// The states array
        /// </summary>
        private volatile int[] states;

        #region Starts and ends

        //private volatile int producerStart;

        /// <summary>
        /// The producer queue end
        /// </summary>
        private volatile int producerEnd;

        //private volatile int consumerStart;

        /// <summary>
        /// The consumer queue end
        /// </summary>
        private volatile int consumerEnd;
        
        //public int ProducerStart { get { return Interlocked.CompareExchange(ref producerStart, 0, 0); } }

        /// <summary>
        /// Returns the producer queue end index
        /// </summary>
        public int ProducerEnd { get { return Interlocked.CompareExchange(ref producerEnd, 0, 0); } }

        //public int ConsumerStart { get { return Interlocked.CompareExchange(ref producerStart, 0, 0); } }

        /// <summary>
        /// Returns the consumer queue end index
        /// </summary>
        public int ConsumerEnd { get { return Interlocked.CompareExchange(ref consumerEnd, 0, 0); } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentGenericCircularBuffer"/> class
        /// </summary>
        /// <param name="contents">The contents array</param>
        /// <param name="states">The states array</param>
        public ConcurrentGenericCircularBuffer(
            TValue[] contents,
            int[] states)
        {
            this.contents = contents;

            this.states = states;
            
            //producerStart = 0;
            
            producerEnd = 0;

            //consumerStart = 0;

            consumerEnd = 0;
        }

        #region Get

        /// <summary>
        /// Retrieves the state of the buffer element by index in thread safe manner
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The state of the buffer element by index</returns>
        public EBufferElementState GetState(int index)
        {
            return (EBufferElementState)Interlocked.CompareExchange(ref states[index], 0, 0);
        }

        /// <summary>
        /// Retrieves the current buffer element value by index in thread safe manner
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The current buffer element value by index</returns>
        public TValue GetValue(int index)
        {
            return Interlocked.CompareExchange<TValue>(
                ref contents[index], 
                default(TValue), 
                default(TValue));
        }

        #endregion
        
        #region Produce

        /// <summary>
        /// Attempts to write the value to the buffer
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>Whether the write was successful</returns>
        public bool TryProduce(TValue value)
        {
            SpinWait spin = new SpinWait();

            return TryAllocateProducer(spin, out int index) && TryProduce(index, value);
        }

        /// <summary>
        /// Attempts to claim the next buffer element in the producer queue for writing and retries if the producer queue index changed during the attempt
        /// </summary>
        /// <param name="spin">The spinlock</param>
        /// <param name="index">The current producer queue end index</param>
        /// <returns>Whether the allocation was successful</returns>
        private bool TryAllocateProducer(SpinWait spin, out int index)
        {
            int lastIndex = -1;

            while (true)
            {
                //Try claiming the slot at the end of the producer queue
                if (TryAllocateProducer(out index))
                    return true;

                //If we try to claim the same slot twice then the producer queue has met the consumer queue and the buffer is full
                if (index == lastIndex)
                {
                    index = -1;

                    return false;
                }

                //Otherwise it's some other thread that has snatched the slot, just spin and try claiming again
                lastIndex = index;
                
                spin.SpinOnce();
            }
        }

        /// <summary>
        /// Attempts to claim the next buffer element in the producer queue for writing
        /// </summary>
        /// <param name="currentProducerEnd">The current producer queue end index</param>
        /// <returns>Whether the allocation was successful</returns>
        private bool TryAllocateProducer(out int currentProducerEnd)
        {
            #region Read producer queue end index

            //Read the current producer queue end index
            currentProducerEnd = Interlocked.CompareExchange(ref producerEnd, 0, 0);

            #endregion
            
            #region Validate slot state

            //Read the state of the current producer queue end
            var currentState = Interlocked.CompareExchange(ref states[currentProducerEnd], 0, 0);
            
            //If it's not vacant then produce failure
            if (currentState != STATE_VACANT)
            {
                return false;
            }

            #endregion

            #region Claim the slot at the end of the queue

            //Try to write new descriptor values
            var updatedState = Interlocked.CompareExchange(
                    ref states[currentProducerEnd],
                    STATE_ALLOCATED_FOR_PRODUCER,
                    STATE_VACANT);
            
            //Proceed only if succeeded
            if (updatedState != currentState)
            {
                return false;
            }
            
            #endregion

            #region Increment producer queue end index and write back

            int newProducerEnd = IncrementAndWrap(currentProducerEnd);
            
            var updatedProducerEnd = Interlocked.CompareExchange(
                ref producerEnd,
                newProducerEnd,
                currentProducerEnd);

            #endregion
            
            return true;
        }

        /// <summary>
        /// Attempts to write the value into buffer element allocated for writing and change its state to 'filled' so that it can be read from by consumers
        /// </summary>
        /// <param name="index">The buffer element index</param>
        /// <param name="value">The value</param>
        /// <returns>Whether the write was successful</returns>
        private bool TryProduce(int index, TValue value)
        {
            //Write value
            Interlocked.Exchange<TValue>(
                ref contents[index],
                value);

            //Update state
            int updatedState = Interlocked.CompareExchange(
                ref states[index],
                STATE_FILLED,
                STATE_ALLOCATED_FOR_PRODUCER);

            return updatedState == STATE_ALLOCATED_FOR_PRODUCER;
        }
        
        #endregion

        #region Consume

        /// <summary>
        /// Attempts to read the value from the buffer
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>Whether the read was successful</returns>
        public bool TryConsume(out TValue value)
        {
            value = default(TValue);

            SpinWait spin = new SpinWait();

            return TryAllocateConsumer(spin, out int index) && TryConsume(index, out value);
        }

        /// <summary>
        /// Attempts to claim the next buffer element in the consumer queue for reading and retries if the consumer queue index changed during the attempt
        /// </summary>
        /// <param name="spin">The spinlock</param>
        /// <param name="index">The current consumer queue end index</param>
        /// <returns>Whether the allocation was successful</returns>
        private bool TryAllocateConsumer(SpinWait spin, out int index)
        {
            int lastIndex = -1;

            while (true)
            {
                //Try claiming the slot at the end of the consumer queue
                if (TryAllocateConsumer(out index))
                    return true;

                //If we try to claim the same slot twice then the consumer queue has met the producer queue and the buffer is empty
                if (index == lastIndex)
                {
                    index = -1;

                    return false;
                }

                //Otherwise it's some other thread that has snatched the slot, just spin and try claiming again
                lastIndex = index;
                
                spin.SpinOnce();
            }
        }

        /// <summary>
        /// Attempts to claim the next buffer element in the consumer queue for reading
        /// </summary>
        /// <param name="currentConsumerEnd">The current consumer queue end index</param>
        /// <returns>Whether the allocation was successful</returns>
        private bool TryAllocateConsumer(out int currentConsumerEnd)
        {
            #region Read consumer queue end index

            //Read the current consumer queue end index
            currentConsumerEnd = Interlocked.CompareExchange(ref consumerEnd, 0, 0);

            #endregion
            
            #region Validate slot state

            //Read the state of the current consumer queue end
            var currentState = Interlocked.CompareExchange(ref states[currentConsumerEnd], 0, 0);
            
            //If it's not filled then produce failure
            if (currentState != STATE_FILLED)
            {
                return false;
            }

            #endregion

            #region Claim the slot at the end of the queue

            //Try to write new descriptor values
            var updatedState = Interlocked.CompareExchange(
                    ref states[currentConsumerEnd],
                    STATE_ALLOCATED_FOR_CONSUMER,
                    STATE_FILLED);
            
            //Proceed only if succeeded
            if (updatedState != currentState)
            {
                return false;
            }
            
            #endregion

            #region Increment consumer queue end index and write back

            int newConsumerEnd = IncrementAndWrap(currentConsumerEnd);
            
            var updatedConsumerEnd = Interlocked.CompareExchange(
                ref consumerEnd,
                newConsumerEnd,
                currentConsumerEnd);

            #endregion
            
            return true;
        }

        /// <summary>
        /// Attempts to read the value from the buffer element allocated for reading and change its state to 'vacant' so that it can be written to by producers
        /// </summary>
        /// <param name="index">The buffer element index</param>
        /// <param name="value">The value</param>
        /// <returns>Whether the read was successful</returns>
        private bool TryConsume(int index, out TValue value)
        {
            //Read value
            value = Interlocked.CompareExchange<TValue>(
                ref contents[index],
                default(TValue),
                default(TValue));

            //Update state
            int updatedState = Interlocked.CompareExchange(
                ref states[index],
                STATE_VACANT,
                STATE_ALLOCATED_FOR_CONSUMER);

            return updatedState == STATE_ALLOCATED_FOR_CONSUMER;
        }

        #endregion
        
        /// <summary>
        /// Increments the queue index and wraps it around if index gets outside the contents array length
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The incremented index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int IncrementAndWrap(int index)
        {
            return (++index) % contents.Length;
        }
    }   
}