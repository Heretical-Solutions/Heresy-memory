namespace HereticalSolutions.Memory
{
    public unsafe struct MemoryAllocationDescriptor
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
        /// Allocation status
        /// </summary>
        public EAllocationStatus Status;
        
        /// <summary>
        /// Create the allocation. Its contents are initially undefined.
        /// </summary>
        /// <param name="memoryPointer">Pointer to the unmanaged heap memory the array is stored in</param>
        /// <param name="memorySize">Unmanaged memory size in bytes</param>
        public MemoryAllocationDescriptor(
            byte* memoryPointer,
            int memorySize)
        {
            MemoryPointer = memoryPointer;
            
            MemorySize = memorySize;
            
            Status = EAllocationStatus.FREE;
        }
    }
}