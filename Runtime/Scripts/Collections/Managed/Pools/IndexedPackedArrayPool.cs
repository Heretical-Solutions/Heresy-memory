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
		protected IndexedPackedArray<T> packedArray;

		public IndexedPackedArray<T> Contents { get => packedArray; }

		public void UpdateContents(IndexedPackedArray<T> newContents)
		{
			packedArray = newContents;
		}

		protected Action<IndexedPackedArrayPool<T>> resizeDelegate;

		public AllocationCommand<IPoolElement<T>> AllocationCommand { get; private set; }

		public IndexedPackedArrayPool(
			IndexedPackedArray<T> packedArray,
			Action<IndexedPackedArrayPool<T>> resizeDelegate,
			AllocationCommand<IPoolElement<T>> allocationCommand)
		{
			this.packedArray = packedArray;

			this.resizeDelegate = resizeDelegate;

			AllocationCommand = allocationCommand;
		}

		public IPoolElement<T> Pop()
		{
			if (!packedArray.HasFreeSpace)
				resizeDelegate(this);

			IPoolElement<T> result = packedArray.Pop();

			return result;
		}

		public void Push(IPoolElement<T> instance)
		{
			packedArray.Push(instance);
		}
	}
}