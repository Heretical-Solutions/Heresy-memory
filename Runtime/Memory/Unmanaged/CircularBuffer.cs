using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Memory.Unmanaged
{
    //Courtesy of https://github.com/joaoportela/CircularBuffer-CSharp/blob/master/CircularBuffer/CircularBuffer.cs
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CircularBuffer
    {
        /// <summary>
        /// Pointer to the unmanaged heap memory the array is stored in
        /// </summary>
        public byte* MemoryPointer;
        
        /// <summary>
        /// Unmanaged memory size in bytes
        /// </summary>
        public int MemorySize;
        
        /// <summary>
        /// The size in bytes of particular element in array
        /// </summary>
        public int ElementSize;
        
        /// <summary>
        /// The maximum amount of elements allowed in the array
        /// </summary>
        public int ElementCapacity;
        
        /// <summary>
        /// Index of the first element in buffer
        /// </summary>
        private int Start;

        /// <summary>
        /// Index after the last element in the buffer
        /// </summary>
        private int End;
        
        /// <summary>
        /// The amount of elements currently allocated in the buffer
        /// </summary>
        public int Count;
        
        /// <summary>
        /// Boolean indicating if Circular is at full capacity.
        /// Adding more elements when the buffer is full will
        /// cause elements to be removed from the other end
        /// of the buffer.
        /// </summary>
        public bool IsFull
        {
            get
            {
                return Count == ElementCapacity;
            }
        }

        /// <summary>
        /// True if has no elements.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }
        
        /// <summary>
        /// Create the buffer. Its elements are initially undefined.
        /// </summary>
        /// <param name="memoryPointer">Pointer to the unmanaged heap memory the array is stored in</param>
        /// <param name="memorySize">Unmanaged memory size in bytes</param>
        /// <param name="elementSize">The size of one element of the array in bytes</param>
        /// <param name="elementCapacity">Number of elements in the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CircularBuffer(
            byte* memoryPointer,
            int memorySize,
            int elementSize,
            int elementCapacity)
        {
            MemoryPointer = memoryPointer;
            
            MemorySize = memorySize;
            
            ElementSize = elementSize;
            
            ElementCapacity = elementCapacity;
            
            Start = 0;
            
            End = 0;
            
            Count = 0;
        }
        
        public void* this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                /*
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                }
                
                if (index >= Size)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, Size));
                }
                */
                
                int actualIndex = InternalIndex(index);
                
                return MemoryPointer + ElementSize * actualIndex;
            }
        }

        /// <summary>
        /// Pushes a new element to the back of the buffer. Back()/this[Size-1]
        /// will now return this element.
        /// 
        /// When the buffer is full, the element at Front()/this[0] will be 
        /// popped to allow for this new element to fit.
        /// </summary>
        /// <param name="item">Item to push to the back of the buffer</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* PushBack()
        {
            var result = this[End];
            
            End = Increment(End);
            
            if (IsFull)
            {
                Start = Increment(End);
            }
            else
            {
                ++Count;
            }
            
            return result;
        }
        
        /// <summary>
        /// Removes the element at the front of the buffer. Decreasing the 
        /// Buffer size by 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* PopFront()
        {
            ThrowIfEmpty("Cannot take elements from an empty buffer.");
            
            var result = this[Start];
            
            Start = Increment(Start);
            
            --Count;
            
            return result;
        }

        /// <summary>
        /// Clears the contents of the array. Size = 0, Capacity is unchanged.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Start = 0;
            
            End = 0;
            
            Count = 0;
            
            //Optional
            //Buffer.Clear(memoryChunk.Memory, 0, memoryChunk.Size);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Increments the provided index variable by one, wrapping
        /// around if necessary.
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Increment(int index)
        {
            if (++index == ElementCapacity)
            {
                index = 0;
            }
            
            return index;
        }

        /// <summary>
        /// Decrements the provided index variable by one, wrapping
        /// around if necessary.
        /// </summary>
        /// <param name="index"></param>
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

        /// <summary>
        /// Converts the index in the argument to an index in buffer
        /// </summary>
        /// <returns>
        /// The transformed index.
        /// </returns>
        /// <param name='index'>
        /// External index.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int InternalIndex(int index)
        {
            return Start + (
                (index < (ElementCapacity - Start))
                    ? index
                    : index - ElementCapacity);
        }
    }   
}