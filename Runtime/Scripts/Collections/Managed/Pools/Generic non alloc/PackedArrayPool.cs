using System;
using HereticalSolutions.Allocations;

namespace HereticalSolutions.Collections.Managed
{
	public class PackedArrayPool<T>
		: INonAllocPool<T>,
		  IResizable<IPoolElement<T>>, //used by CollectionFactory to resize
		  IContentsRetrievable<IndexedPackedArray<T>> //used by CollectionFactory to resize
	{
		protected IndexedPackedArray<T> packedArray;

		#region IContentsRetrievable

		public IndexedPackedArray<T> Contents { get => packedArray; }

		#endregion

		#region IResizable

		public AllocationCommand<IPoolElement<T>> ResizeAllocationCommand { get; private set; }

		protected Action<PackedArrayPool<T>> resizeDelegate;

		public void Resize()
		{
			resizeDelegate(this);
		}

		#endregion

		protected Action<IPoolElement<T>> notifyAllocationDelegate;

		public PackedArrayPool(
			IndexedPackedArray<T> packedArray,
			Action<PackedArrayPool<T>> resizeDelegate,
			AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
			Action<IPoolElement<T>> notifyAllocationDelegate = null)
		{
			this.packedArray = packedArray;

			this.resizeDelegate = resizeDelegate;

			this.notifyAllocationDelegate = notifyAllocationDelegate;

			ResizeAllocationCommand = resizeAllocationCommand;

			for (int i = 0; i < packedArray.Capacity; i++)
				notifyAllocationDelegate?.Invoke(packedArray.ElementAt(i));
		}

		#region INonAllocPool

		public IPoolElement<T> Pop()
		{
			if (!packedArray.HasFreeSpace)
			{
				int previousCapacity = packedArray.Capacity;

				resizeDelegate(this);

				int newCapacity = packedArray.Capacity;

				for (int i = previousCapacity; i < newCapacity; i++)
					notifyAllocationDelegate?.Invoke(packedArray.ElementAt(i));
			}

			IPoolElement<T> result = packedArray.Pop();

			return result;
		}

		public void Push(IPoolElement<T> instance)
		{
			packedArray.Push(instance);
		}

		#endregion
	}
}