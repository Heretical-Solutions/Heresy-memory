using System;
using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Internal;

namespace HereticalSolutions.Collections.Managed
{
	public class IndexedPackedArrayPool<T> 
		: INonAllocPool<T>,
		  IResizable<IPoolElement<T>>,
		  IAppendable<IPoolElement<T>>,
		  ITopUppable<T>,
		  IContentsRetrievable<IndexedPackedArray<T>>
	{
		protected IndexedPackedArray<T> packedArray;

		#region IContentsRetrievable

		public IndexedPackedArray<T> Contents { get => packedArray; }

		#endregion

		#region IResizable

		public AllocationCommand<IPoolElement<T>> ResizeAllocationCommand { get; private set; }

		protected Action<IndexedPackedArrayPool<T>> resizeDelegate;

		public void Resize()
		{
			resizeDelegate(this);
		}

		#endregion

		#region IAppendable

		public AllocationCommand<IPoolElement<T>> AppendAllocationCommand { get; private set; }

		protected Func<IndexedPackedArrayPool<T>, IPoolElement<T>> appendDelegate;

		public IPoolElement<T> Append()
		{
			return appendDelegate(this);
		}

		#endregion

		#region ITopUppable

		public Func<T> TopUpAllocationDelegate { get; private set; }

		public void TopUp(IPoolElement<T> element)
		{
			element.Value = TopUpAllocationDelegate();
		}

		#endregion

		public IndexedPackedArrayPool(
			IndexedPackedArray<T> packedArray,
			Action<IndexedPackedArrayPool<T>> resizeDelegate,
			AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
			Func<IndexedPackedArrayPool<T>, IPoolElement<T>> appendDelegate,
			AllocationCommand<IPoolElement<T>> appendAllocationCommand,
			Func<T> topUpAllocationDelegate)
		{
			this.packedArray = packedArray;

			this.resizeDelegate = resizeDelegate;

			this.appendDelegate = appendDelegate;

			ResizeAllocationCommand = resizeAllocationCommand;

			AppendAllocationCommand = appendAllocationCommand;

			TopUpAllocationDelegate = topUpAllocationDelegate;
		}

		#region INonAllocPool

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

		#endregion
	}
}