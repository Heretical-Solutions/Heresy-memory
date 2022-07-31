using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Memory
{
    /// <summary>
    /// An array stored in the unmanaged heap with Pop and Push methods
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct GenericPackedArray<TValue> where TValue : unmanaged
    {
        /// <summary>
        /// Pointer to the unmanaged heap memory the array is stored in
        /// </summary>
        public TValue* MemoryPointer;
        
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
        /// The amount of elements currently allocated in the array
        /// </summary>
        public int Count;
        
        /// <summary>
        /// Create the array. Its elements are initially undefined.
        /// </summary>
        /// <param name="memoryPointer">Pointer to the unmanaged heap memory the array is stored in</param>
        /// <param name="memorySize">Unmanaged memory size in bytes</param>
        /// <param name="elementSize">The size of one element of the array in bytes</param>
        /// <param name="elementCapacity">Number of elements in the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GenericPackedArray(
            TValue* memoryPointer,
            int memorySize,
            int elementSize,
            int elementCapacity)
        {
            MemoryPointer = memoryPointer;
            
            MemorySize = memorySize;
            
            ElementSize = elementSize;
            
            ElementCapacity = elementCapacity;
            
            Count = 0;
        }
        
        /// <summary>
        /// Is given index valid for the array
        /// </summary>
        /// <param name="index">Target index</param>
        /// <returns>Is index valid</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IndexValid(int index)
        {
            return (index > -1) && (index < ElementCapacity);
        }
    
        /// <summary>
        /// Get a pointer to an element in the array
        /// </summary>
        /// <param name="index">Index of the element to get a pointer to</param>
        public ref TValue this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref MemoryPointer[index];
            }
        }
        
        /// <summary>
        /// Get a pointer to an element in the array
        /// </summary>
        /// <param name="index">Index of the element to get a pointer to</param>
        public ref TValue this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref MemoryPointer[index];
            }
        }
        
        /// <summary>
        /// Get a pointer to an element in the array
        /// </summary>
        /// <param name="index">Index of the element to get a pointer to</param>
        /// <returns>Element in the array at specified index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue* Get(int index)
        {
            return MemoryPointer + index;
        }
        
        /// <summary>
        /// Get a pointer to an element in the array
        /// </summary>
        /// <param name="index">Index of the element to get a pointer to</param>
        /// <returns>Element in the array at specified index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue* Get(uint index)
        {
            return MemoryPointer + index;
        }
        
        /// <summary>
        /// Get the element's index in the array
        /// </summary>
        /// <param name="element">Target element</param>
        /// <returns>Element index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(TValue* element)
        {
            var distance = element - MemoryPointer;
            
            return (int)distance;
        }
        
        /// <summary>
        /// Does the array have any free elements?
        /// </summary>
        public bool HasFreeSpace
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Count < ElementCapacity;
            }
        }
        
        /// <summary>
        /// Pop a free element from the array
        /// </summary>
        /// <returns>Free element</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TValue Pop()
        {
            if (!HasFreeSpace)
                throw new Exception("[PackedArray] No more space");
            
            ref TValue result = ref this[Count];
            
            Count++;
            
            return ref result;
        }
        
        /// <summary>
        /// Push an element back to the array
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Index of the element that took the place of the pushed element</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Push(int index)
        {
            if (index < 0
                || index >= Count)
                throw new Exception($"[PackedArray] Invalid index {index}");
            
            if (index == Count - 1)
            {
                Count--;
                
                return -1;
            }
            
            if (index != Count - 1)
                Buffer.MemoryCopy(
                    (void*)(MemoryPointer + Count - 1),
                    (void*)(MemoryPointer + index),
                    ElementSize,
                    ElementSize);
            
            int returnValue = Count - 1;
            
            Count--;
                
            return returnValue;
        }
        
        /// <summary>
        /// Push an element back to the array
        /// </summary>
        /// <param name="index">Element index</param>
        /// <returns>Index of the element that took the place of the pushed element</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Push(uint index)
        {
            if (index < 0
                || index >= Count)
                throw new Exception($"[PackedArray] Invalid index {index}");
            
            if (index == Count - 1)
            {
                Count--;
                
                return -1;
            }
            
            if (index != Count - 1)
                Buffer.MemoryCopy(
                    (void*)(MemoryPointer + Count - 1),
                    (void*)(MemoryPointer + index),
                    ElementSize,
                    ElementSize);
            
            int returnValue = Count - 1;
            
            Count--;
                
            return returnValue;
        }
        
        /// <summary>
        /// Push an element back to the array
        /// </summary>
        /// <param name="element">Element to push</param>
        /// <returns>Index of the element that took the place of the pushed element</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Push(TValue* element)
        {
            int elementIndex = IndexOf(element);
            
            return Push(elementIndex);
        }
    }   
}