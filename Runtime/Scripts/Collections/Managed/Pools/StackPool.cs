using System;
using System.Collections.Generic;
using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Internal;

namespace HereticalSolutions.Collections.Managed
{
	public class StackPool<T> 
		: IPool<T>,
		  IResizable<T>,
		  IContentsRetrievable<Stack<T>>
	{
		protected Stack<T> pool;

		public Stack<T> Contents { get => pool; }

		public void UpdateContents(Stack<T> newContents)
		{
			pool = newContents;
		}

		protected Action<StackPool<T>> resizeDelegate;

		public AllocationCommand<T> AllocationCommand { get; private set; }

		public StackPool(
			Stack<T> pool,
			Action<StackPool<T>> resizeDelegate,
			AllocationCommand<T> allocationCommand)
		{
			this.pool = pool;

			this.resizeDelegate = resizeDelegate;

			AllocationCommand = allocationCommand;
		}

		public T Pop()
		{
			T result = default(T);

			if (pool.Count != 0)
			{
				result = pool.Pop();
			}
			else
			{
				resizeDelegate(this);

				result = pool.Pop();
			}

			OnBeforePop(result);

			return result;
		}

		protected virtual void OnBeforePop(T instance)
		{
		}

		public void Push(T instance)
		{
			OnBeforePush(instance);

			pool.Push(instance);

			OnAfterPush(instance);
		}

		protected virtual void OnBeforePush(T instance)
		{
		}

		protected virtual void OnAfterPush(T instance)
		{
		}
	}
}