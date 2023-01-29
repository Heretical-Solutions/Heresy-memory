using System;
using HereticalSolutions.Collections.Allocations;

namespace HereticalSolutions.Collections.Managed
{
	public class PackedArrayPool<T>
		: INonAllocPool<T>,
		  IResizable<IPoolElement<T>>, //used by CollectionFactory to resize
		  IContentsRetrievable<IndexedPackedArray<T>>, //used by CollectionFactory to resize
		  ITopUppable<T>
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

		#region ITopUppable

		private Func<T> topUpAllocationDelegate;

		public void TopUp(IPoolElement<T> element)
		{
			element.Value = topUpAllocationDelegate.Invoke();
		}

		#endregion

		public PackedArrayPool(
			IndexedPackedArray<T> packedArray,
			Action<PackedArrayPool<T>> resizeDelegate,
			AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
			Func<T> topUpAllocationDelegate)
		{
			this.packedArray = packedArray;

			this.resizeDelegate = resizeDelegate;

			this.topUpAllocationDelegate = topUpAllocationDelegate;

			ResizeAllocationCommand = resizeAllocationCommand;
		}

		#region INonAllocPool

		public IPoolElement<T> Pop()
		{
			if (!packedArray.HasFreeSpace)
			{
				int previousCapacity = packedArray.Capacity;

				resizeDelegate(this);

				int newCapacity = packedArray.Capacity;
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