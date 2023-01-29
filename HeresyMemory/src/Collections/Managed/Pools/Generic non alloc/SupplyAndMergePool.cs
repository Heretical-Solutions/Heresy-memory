using System;
using HereticalSolutions.Collections.Allocations;

namespace HereticalSolutions.Collections.Managed
{
	public class SupplyAndMergePool<T> 
		: INonAllocPool<T>,
		  IAppendable<IPoolElement<T>>,
		  ITopUppable<T>
	{
		protected IndexedPackedArray<T> baseArray;

		protected IndexedPackedArray<T> supplyArray;

		#region IAppendable

		public AllocationCommand<IPoolElement<T>> AppendAllocationCommand { get; private set; }

		public IPoolElement<T> Append()
		{
			if (!supplyArray.HasFreeSpace)
			{
				MergeSupplyIntoBase();
			}

			IPoolElement<T> result = supplyArray.Pop();

			return result;
		}

		#endregion

		protected Action<IndexedPackedArray<T>, IndexedPackedArray<T>, AllocationCommand<IPoolElement<T>>> mergeDelegate;

		protected void MergeSupplyIntoBase()
		{
			mergeDelegate.Invoke(baseArray, supplyArray, AppendAllocationCommand);
		}

		public void TopUpAndMerge()
		{
			for (int i = supplyArray.Count; i < supplyArray.Capacity; i++)
				TopUp(supplyArray.ElementAt(i));

			MergeSupplyIntoBase();
		}

		#region ITopUppable

		private Func<T> topUpAllocationDelegate;

		public void TopUp(IPoolElement<T> element)
		{
			element.Value = topUpAllocationDelegate.Invoke();
		}

		#endregion

		public SupplyAndMergePool(
			IndexedPackedArray<T> baseArray,
			IndexedPackedArray<T> supplyArray,
			AllocationCommand<IPoolElement<T>> appendAllocationCommand,
			Action<IndexedPackedArray<T>, IndexedPackedArray<T>, AllocationCommand<IPoolElement<T>>> mergeDelegate,
			Func<T> topUpAllocationDelegate)
		{
			this.baseArray = baseArray;

			this.supplyArray = supplyArray;

			this.mergeDelegate = mergeDelegate;

			this.topUpAllocationDelegate = topUpAllocationDelegate;

			AppendAllocationCommand = appendAllocationCommand;
		}

		#region INonAllocPool

		public IPoolElement<T> Pop()
		{
			if (!baseArray.HasFreeSpace)
			{
				TopUpAndMerge();
			}

			IPoolElement<T> result = baseArray.Pop();

			return result;
		}

		public void Push(IPoolElement<T> instance)
		{
			var instanceIndex = ((IIndexed)instance).Index;

			if (instanceIndex > -1
				&& instanceIndex < supplyArray.Count
				&& supplyArray[instanceIndex] == instance)
			{
				TopUpAndMerge();
			}

			baseArray.Push(instance);
		}

		#endregion
	}
}