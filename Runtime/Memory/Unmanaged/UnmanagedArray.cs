using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Memory
{
    /// <summary>
    /// An array stored in the unmanaged heap
    /// Courtesy of http://JacksonDunstan.com/articles/3740
    /// Courtesy of https://github.com/bepu/bepuphysics2/blob/master/BepuUtilities/Memory/Buffer.cs
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UnmanagedArray
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
        /// Create the array. Its elements are initially undefined.
        /// </summary>
        /// <param name="memoryPointer">Pointer to the unmanaged heap memory the array is stored in</param>
        /// <param name="memorySize">Unmanaged memory size in bytes</param>
        /// <param name="elementSize">The size of one element of the array in bytes</param>
        /// <param name="elementCapacity">Number of elements in the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedArray(
            byte* memoryPointer,
            int memorySize,
            int elementSize,
            int elementCapacity)
        {
            MemoryPointer = memoryPointer;
            
            MemorySize = memorySize;
            
            ElementSize = elementSize;
            
            ElementCapacity = elementCapacity;
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
        public void* this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return MemoryPointer + ElementSize * index;
            }
        }
        
        /// <summary>
        /// Get a pointer to an element in the array
        /// </summary>
        /// <param name="index">Index of the element to get a pointer to</param>
        public void* this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return MemoryPointer + ElementSize * index;
            }
        }
        
        /// <summary>
        /// Get a pointer to an element in the array
        /// </summary>
        /// <param name="index">Index of the element to get a pointer to</param>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Element in the array at specified index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* Get<T>(int index) where T : unmanaged
        {
            return (T*)(MemoryPointer + ElementSize * index);
        }
        
        /// <summary>
        /// Get a pointer to an element in the array
        /// </summary>
        /// <param name="index">Index of the element to get a pointer to</param>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Element in the array at specified index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* Get<T>(uint index) where T : unmanaged
        {
            return (T*)(MemoryPointer + ElementSize * index);
        }
        
        /// <summary>
        /// Get the element's index in the array
        /// </summary>
        /// <param name="element">Target element</param>
        /// <returns>Element index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(void* element)
        {
            long distance = (byte*)element - MemoryPointer;
            
            return (int)(distance / ElementSize);
        }
        
        /// <summary>
        /// Get the element's index in the array
        /// </summary>
        /// <param name="element">Target element</param>
        /// <returns>Element index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf<T>(T* element) where T : unmanaged
        {
            long distance = (byte*)element - MemoryPointer;
            
            return (int)(distance / ElementSize);
        }
    }   
}