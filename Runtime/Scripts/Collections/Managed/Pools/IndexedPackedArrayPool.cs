using System;
using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Internal;

namespace HereticalSolutions.Collections.Managed
{
	public class IndexedPackedArrayPool<T> 
		: INonAllocPool<T>,
		  IResizable<IPoolElement<T>>,
		  IContentsRetrievable<IndexedPackedArray<T>>
	{
		protected IndexedPackedArray<T> pool;

		public IndexedPackedArray<T> Contents { get => pool; }

		public void UpdateContents(IndexedPackedArray<T> newContents)
		{
			pool = newContents;
		}

		protected Action<IndexedPackedArrayPool<T>> resizeDelegate;

		public AllocationCommand<IPoolElement<T>> AllocationCommand { get; private set; }

		public IndexedPackedArrayPool(
			IndexedPackedArray<T> pool,
			Action<IndexedPackedArrayPool<T>> resizeDelegate,
			AllocationCommand<IPoolElement<T>> allocationCommand)
		{
			this.pool = pool;

			this.resizeDelegate = resizeDelegate;

			AllocationCommand = allocationCommand;
		}

		public IPoolElement<T> Pop()
		{
			if (!pool.HasFreeSpace)
				resizeDelegate(this);

			IPoolElement<T> result = pool.Pop();

			return result;
		}

		public void Push(IPoolElement<T> instance)
		{
			pool.Push(instance);
		}
	}
}