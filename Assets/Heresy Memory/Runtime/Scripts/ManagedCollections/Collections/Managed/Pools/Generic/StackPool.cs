using System;
using System.Collections.Generic;
using HereticalSolutions.Collections.Allocations;

namespace HereticalSolutions.Collections.Managed
{
	public class StackPool<T> 
		: IPool<T>,
		  IResizable<T>,
		  IContentsRetrievable<Stack<T>>
	{
		protected Stack<T> pool;

		#region IContentsRetrievable

		public Stack<T> Contents { get => pool; }

		#endregion

		public void UpdateContents(Stack<T> newContents)
		{
			pool = newContents;
		}

		#region IResizable

		public AllocationCommand<T> ResizeAllocationCommand { get; private set; }

		protected Action<StackPool<T>> resizeDelegate;

		public void Resize()
		{
			resizeDelegate(this);
		}

		#endregion

		public StackPool(
			Stack<T> pool,
			Action<StackPool<T>> resizeDelegate,
			AllocationCommand<T> allocationCommand)
		{
			this.pool = pool;

			this.resizeDelegate = resizeDelegate;

			ResizeAllocationCommand = allocationCommand;
		}

		#region IPool

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
			return result;
		}

		public void Push(T instance)
		{
			pool.Push(instance);
		}

		#endregion
	}
}