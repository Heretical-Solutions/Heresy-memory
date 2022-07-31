using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Memory
{
    /// <summary>
    /// A chunk of unmanaged memory
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MemoryChunk
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
        /// Create the chunk. Its contents are initially undefined.
        /// </summary>
        /// <param name="memoryPointer">Pointer to the unmanaged heap memory the array is stored in</param>
        /// <param name="memorySize">Unmanaged memory size in bytes</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryChunk(
            byte* memoryPointer,
            int memorySize)
        {
            MemoryPointer = memoryPointer;
            
            MemorySize = memorySize;
        }
    }   
}