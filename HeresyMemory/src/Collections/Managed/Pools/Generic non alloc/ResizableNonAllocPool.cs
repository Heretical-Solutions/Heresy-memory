using System;

using HereticalSolutions.Collections.Allocations;

namespace HereticalSolutions.Collections.Managed
{
	public class ResizableNonAllocPool<T>
		: INonAllocPool<T>,
		  IResizable<IPoolElement<T>>, //used by CollectionFactory to resize
		  //IModifiable<IndexedPackedArray<T>>, //used by CollectionFactory to resize
		  IModifiable<INonAllocPool<T>>, //used by CollectionFactory to resize
		  ITopUppable<T>
	{
		//protected IndexedPackedArray<T> packedArray;
		protected INonAllocPool<T> nonAllocPool;
		protected IModifiable<IPoolElement<T>[]> poolAsModifiable;
		protected IFixedSizeCollection<IPoolElement<T>> poolAsFixedSizeCollection;

		public ResizableNonAllocPool(
			//IndexedPackedArray<T> packedArray,
			INonAllocPool<T> nonAllocPool,
			Action<ResizableNonAllocPool<T>> resizeDelegate,
			AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
			Func<T> topUpAllocationDelegate)
		{
			//this.packedArray = packedArray;
			this.nonAllocPool = nonAllocPool;
			poolAsModifiable = (IModifiable<IPoolElement<T>[]>)nonAllocPool;
			poolAsFixedSizeCollection = (IFixedSizeCollection<IPoolElement<T>>)nonAllocPool;

			this.resizeDelegate = resizeDelegate;

			this.topUpAllocationDelegate = topUpAllocationDelegate;

			ResizeAllocationCommand = resizeAllocationCommand;
		}
		
		#region IModifiable

		//public IndexedPackedArray<T> Contents { get => packedArray; }
		public INonAllocPool<T> Contents { get => nonAllocPool; }
		
		//public void UpdateContents(IndexedPackedArray<T> newContents)
		public void UpdateContents(INonAllocPool<T> newContents)
		{
			//packedArray = newContents;
			nonAllocPool = newContents;
		}
		
		public void UpdateCount(int newCount)
		{
			//packedArray.UpdateCount(newCount);
			poolAsModifiable.UpdateCount(newCount);
		}

		#endregion

		#region IResizable

		public AllocationCommand<IPoolElement<T>> ResizeAllocationCommand { get; private set; }

		protected Action<ResizableNonAllocPool<T>> resizeDelegate;

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

		#region INonAllocPool

		/*
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
		*/
				
		public IPoolElement<T> Pop()
		{
			if (!nonAllocPool.HasFreeSpace)
			{
				int previousCapacity = poolAsFixedSizeCollection.Capacity;

				resizeDelegate(this);

				int newCapacity = poolAsFixedSizeCollection.Capacity;
			}

			IPoolElement<T> result = nonAllocPool.Pop();

			return result;
		}

		public void Push(IPoolElement<T> instance)
		{
			//packedArray.Push(instance);
			nonAllocPool.Push(instance);
		}
		
		//public bool HasFreeSpace { get { return packedArray.HasFreeSpace; } }
		public bool HasFreeSpace { get { return nonAllocPool.HasFreeSpace; } }

		#endregion
	}
}