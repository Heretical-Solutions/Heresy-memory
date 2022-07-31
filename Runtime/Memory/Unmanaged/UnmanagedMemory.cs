using System;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Memory.Unmanaged
{
    public unsafe class UnmanagedMemory
    {
        /// <summary>
        /// The size of a pointer, in bytes
        /// </summary>
        public static readonly int SizeOfPointer = sizeof(void*);
    
        /// <summary>
        /// The minimum size of an block, in bytes
        /// </summary>
        public static readonly int MinimumPoolBlockSize = SizeOfPointer;
    
        #region Memory operations
    
        /// <summary>
        /// Set a series of bytes to the same value
        /// </summary>
        /// <param name="ptr">Pointer to the first byte to set</param>
        /// <param name="value">Value to set to all the bytes</param>
        /// <param name="count">Number of bytes to set</param>
        public static void Memset(
            void* ptr,
            byte value,
            int count)
        {
            byte* pCur = (byte*)ptr;
            
            for (int i = 0; i < count; ++i)
            {
                *pCur++ = value;
            }
        }
    
        /// <summary>
        /// Allocate unmanaged heap memory and track it
        /// </summary>
        /// <param name="size">Number of bytes of unmanaged heap memory to allocate</param>
        public static IntPtr Alloc(int size)
        {
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            
            return intPtr;
        }
    
        /// <summary>
        /// Allocate unmanaged heap memory filled with zeroes and track it
        /// </summary>
        /// <param name="size">Number of bytes of unmanaged heap memory to allocate</param>
        public static IntPtr Calloc(int size)
        {
            IntPtr intPtr = Alloc(size);
            
            Memset((void*)intPtr, 0, size);
            
            return intPtr;
        }
        
        #endregion
    
        /*
        /// <summary>
        /// Allocate a block of memory from a pool
        /// </summary>
        /// <param name="pool">Pool to allocate from</param>
        public static void* Alloc(UnmanagedMemoryPool* pool)
        {
            void* pRet = pool->Free;
    
            // Return the head of the free list and advance the free list pointer
            pool->Free = *((byte**)pool->Free);
            
            return pRet;
        }
    
        /// <summary>
        /// Allocate a zero-filled block of memory from a pool
        /// </summary>
        /// <param name="pool">Pool to allocate from</param>
        public static void* Calloc(UnmanagedMemoryPool* pool)
        {
            void* ptr = Alloc(pool);
            
            Memset(ptr, 0, pool->BlockSize);
            
            return ptr;
        }
    
        /// <summary>
        /// Allocate a pool of memory. The pool is made up of a fixed number of equal-sized blocks.
        /// Allocations from the pool return one of these blocks.
        /// </summary>
        /// <returns>The allocated pool</returns>
        /// <param name="blockSize">Size of each block, in bytes</param>
        /// <param name="numBlocks">The number of blocks in the pool</param>
        public static UnmanagedMemoryPool AllocPool(
            int blockSize,
            int numBlocks)
        {
            UnmanagedMemoryPool pool = new UnmanagedMemoryPool();
    
            pool.Free = null;
            pool.NumBlocks = numBlocks;
            pool.BlockSize = blockSize;
    
            // Allocate unmanaged memory large enough to fit all the blocks
            pool.Alloc = (byte*)Alloc(blockSize * numBlocks);
    
            // Reset the free list
            FreeAll(&pool);
    
            return pool;
        }
    
        /// <summary>
        /// Free unmanaged heap memory and stop tracking it
        /// </summary>
        /// <param name="ptr">Pointer to the unmanaged heap memory to free. If null, this is a no-op.
        /// </param>
        public static void Free(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    
        /// <summary>
        /// Free a block from a pool
        /// </summary>
        /// <param name="pool">Pool the block is from</param>
        /// <param name="ptr">Pointer to the block to free. If null, this is a no-op.</param>
        public static void Free(UnmanagedMemoryPool* pool, void* ptr)
        {
            // Freeing a null pointer is a no-op, not an error
            if (ptr != null)
            {
                // Insert the block to free at the start of the free list
                void** pHead = (void**)ptr;
                
                *pHead = pool->Free;
                
                pool->Free = pHead;
            }
        }
    
        /// <summary>
        /// Free all the blocks of a pool. This does not free the pool itself, but rather makes all of
        /// its blocks available for allocation again.
        /// </summary>
        /// <param name="pool">Pool whose blocks should be freed</param>
        public static void FreeAll(UnmanagedMemoryPool* pool)
        {
            // Point each block except the last one to the next block. Check their sentinels while we're
            // at it.
            void** pCur = (void**)pool->Alloc;
            
            byte* pNext = pool->Alloc + pool->BlockSize;
            
            for (int i = 0, count = pool->NumBlocks-1; i < count; ++i)
            {
                *pCur = pNext;
                
                pCur = (void**)pNext;
                
                pNext += pool->BlockSize;
            }
    
            // Point the last block to null
            *pCur = default(void*);
    
            // The first block is now the head of the free list
            pool->Free = pool->Alloc;
        }
    
        /// <summary>
        /// Free a pool and all of its blocks. Double-freeing a pool is a no-op.
        /// </summary>
        /// <param name="pool">Pool to free</param>
        public static void FreePool(UnmanagedMemoryPool* pool)
        {
            // Free the unmanaged memory for all the blocks and set to null to allow double-Destroy()
            Free((IntPtr)pool->Alloc);
            
            pool->Alloc = null;
            
            pool->Free = null;
        }
        */
    }   
}