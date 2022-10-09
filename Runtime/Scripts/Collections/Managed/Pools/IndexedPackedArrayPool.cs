using System;
using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Internal;

namespace HereticalSolutions.Collections.Managed
{
	public class IndexedPackedArrayPool<T> 
		: INonAllocPool<T>,
		  IResizable<T>,
		  IContentsRetrievable<IndexedPackedArray<T>>
	{
		protected IndexedPackedArray<T> pool;

		public IndexedPackedArray<T> Contents { get => pool; }

		public void UpdateContents(IndexedPackedArray<T> newContents)
		{
			pool = newContents;
		}

		protected Action<IndexedPackedArrayPool<T>> resizeDelegate;

		public AllocationCommand<T> AllocationCommand { get; private set; }

		public IndexedPackedArrayPool(
			IndexedPackedArray<T> pool,
			Action<IndexedPackedArrayPool<T>> resizeDelegate,
			AllocationCommand<T> allocationCommand)
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

			OnBeforePop(result.Value);

			return result;
		}

		protected virtual void OnBeforePop(T instance)
		{
		}

		public void Push(IPoolElement<T> instance)
		{
			OnBeforePush(instance.Value);

			pool.Push((IndexedContainer<T>)instance);

			OnAfterPush(instance.Value);
		}

		protected virtual void OnBeforePush(T instance)
		{
		}

		protected virtual void OnAfterPush(T instance)
		{
		}
	}
}