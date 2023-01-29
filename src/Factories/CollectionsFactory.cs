using HereticalSolutions.Collections.Allocations;

using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Unmanaged;

using System;
using System.Collections.Generic;

using System.Runtime.InteropServices;

namespace HereticalSolutions.Collections.Factories
{
	public static partial class CollectionsFactory
	{
		#region Stack pool

		public static StackPool<T> BuildStackPool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand)
		{
			var stack = new Stack<T>();

			int initialAmount = -1;

			switch (initialAllocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = initialAllocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}");
			}

			for (int i = 0; i < initialAmount; i++)
				stack.Push(
					initialAllocationCommand.AllocationDelegate());

			return new StackPool<T>(
				stack,
				ResizeStackPool,
				additionalAllocationCommand);
		}

		public static void ResizeStackPool<T>(
			StackPool<T> pool)
		{
			var stack = ((IContentsRetrievable<Stack<T>>)pool).Contents;

			var allocationCommand = ((IResizable<T>)pool).ResizeAllocationCommand;

			int addedCapacity = -1;

			switch (allocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					addedCapacity = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					addedCapacity = allocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}");
			}

			for (int i = 0; i < addedCapacity; i++)
				stack.Push(
					allocationCommand.AllocationDelegate());
		}

		#endregion

		#region Non alloc pools

		public static INonAllocPool<T> BuildPackedArrayPool<T>(
					Func<T> valueAllocationDelegate,
					Func<Func<T>, IPoolElement<T>> containerAllocationDelegate,
					AllocationCommandDescriptor initialAllocation,
					AllocationCommandDescriptor additionalAllocation)
		{
			INonAllocPool<T> packedArrayPool = BuildPackedArrayPool<T>(

				BuildPoolElementAllocationCommand<T>(
					initialAllocation,
					valueAllocationDelegate,
					containerAllocationDelegate),

				BuildPoolElementAllocationCommand<T>(
					additionalAllocation,
					valueAllocationDelegate,
					containerAllocationDelegate),

					valueAllocationDelegate);

			return packedArrayPool;
		}

		public static INonAllocPool<T> BuildSupplyAndMergePool<T>(
			Func<T> valueAllocationDelegate,
			Func<Func<T>, IPoolElement<T>> containerAllocationDelegate,
			AllocationCommandDescriptor initialAllocation,
			AllocationCommandDescriptor additionalAllocation)
		{
			Func<T> nullAllocation = () => { return default(T); };

			INonAllocPool<T> supplyAndMergePool = BuildSupplyAndMergePool<T>(

				BuildPoolElementAllocationCommand<T>(
					initialAllocation,
					valueAllocationDelegate,
					containerAllocationDelegate),

				BuildPoolElementAllocationCommand<T>(
					additionalAllocation,
					nullAllocation,
					containerAllocationDelegate),

				valueAllocationDelegate);

			return supplyAndMergePool;
		}

		#endregion

		#region Supply and merge pool

		public static SupplyAndMergePool<T> BuildSupplyAndMergePool<T>(
			AllocationCommand<IPoolElement<T>> initialAllocationCommand,
			AllocationCommand<IPoolElement<T>> appendAllocationCommand,
			Func<T> topUpAllocationDelegate)
		{
			var basePool = BuildIndexedPackedArray<T>(initialAllocationCommand);

			var supplyPool = BuildIndexedPackedArray<T>(appendAllocationCommand);

			return new SupplyAndMergePool<T>(
				basePool,
				supplyPool,
				appendAllocationCommand,
				MergeIndexedPackedArrays,
				topUpAllocationDelegate);
		}

		#endregion

		#region Packed array pool

		public static PackedArrayPool<T> BuildPackedArrayPool<T>(
			AllocationCommand<IPoolElement<T>> initialAllocationCommand,
			AllocationCommand<IPoolElement<T>> resizeAllocationCommand,
			Func<T> topUpAllocationDelegate)
		{
			var pool = BuildIndexedPackedArray<T>(initialAllocationCommand);

			return new PackedArrayPool<T>(
				pool,
				ResizePackedArrayPool,
				resizeAllocationCommand,
				topUpAllocationDelegate);
		}

		public static void ResizePackedArrayPool<T>(
			PackedArrayPool<T> pool)
		{
			ResizeIndexedPackedArray(
				((IContentsRetrievable<IndexedPackedArray<T>>)pool).Contents,
				pool.ResizeAllocationCommand);
		}

		#endregion

		#region Indexed packed array

		public static IndexedPackedArray<T> BuildIndexedPackedArray<T>(
			AllocationCommand<IPoolElement<T>> allocationCommand)
		{
			int initialAmount = -1;

			switch (allocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = allocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID ALLOCATION COMMAND RULE: {allocationCommand.Descriptor.Rule.ToString()}");
			}

			IPoolElement<T>[] contents = new IPoolElement<T>[initialAmount];

			for (int i = 0; i < initialAmount; i++)
				contents[i] = allocationCommand.AllocationDelegate();

			return new IndexedPackedArray<T>(contents);
		}

		public static void ResizeIndexedPackedArray<T>(
			IndexedPackedArray<T> array,
			AllocationCommand<IPoolElement<T>> allocationCommand)
		{
			int newCapacity = -1;

			switch (allocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					newCapacity = array.Capacity + 1;
					break;

				case EAllocationAmountRule.DOUBLE_AMOUNT:
					newCapacity = Math.Max(array.Capacity, 1) * 2;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					newCapacity = array.Capacity + allocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID ALLOCATION COMMAND RULE FOR INDEXED PACKED ARRAY: {allocationCommand.Descriptor.Rule.ToString()}");
			}

			IPoolElement<T>[] oldContents = ((IContentsRetrievable<IPoolElement<T>[]>)array).Contents;

			IPoolElement<T>[] newContents = new IPoolElement<T>[newCapacity];

			if (newCapacity <= array.Capacity)
			{
				for (int i = 0; i < newCapacity; i++)
					newContents[i] = oldContents[i];
			}
			else
			{
				for (int i = 0; i < array.Capacity; i++)
					newContents[i] = oldContents[i];

				for (int i = array.Capacity; i < newCapacity; i++)
					newContents[i] = allocationCommand.AllocationDelegate();
			}

			((IContentsModifiable<IPoolElement<T>[]>)array).UpdateContents(newContents);
		}

		public static void MergeIndexedPackedArrays<T>(
			IndexedPackedArray<T> receiverArray,
			IndexedPackedArray<T> donorArray,
			AllocationCommand<IPoolElement<T>> donorAllocationCommand)
		{
			IPoolElement<T>[] oldReceiverContents = ((IContentsRetrievable<IPoolElement<T>[]>)receiverArray).Contents;

			IPoolElement<T>[] oldDonorContents = ((IContentsRetrievable<IPoolElement<T>[]>)donorArray).Contents;

			#region Update receiver contents

			int newReceiverCapacity = receiverArray.Capacity + donorArray.Capacity;

			IPoolElement<T>[] newReceiverContents = new IPoolElement<T>[newReceiverCapacity];

			for (int i = 0; i < receiverArray.Capacity; i++)
				newReceiverContents[i] = oldReceiverContents[i];

			for (int i = 0; i < donorArray.Capacity; i++)
			{
				int newIndex = i + receiverArray.Capacity;

				newReceiverContents[newIndex] = oldDonorContents[i];
			}

			if (receiverArray.Capacity == receiverArray.Count)
			{
				for (int i = 0; i < donorArray.Count; i++)
				{
					int newIndex = i + receiverArray.Capacity;

					((IIndexed)newReceiverContents[newIndex]).Index = newIndex;
				}

				/*
				for (int i = donorArray.Count; i < donorArray.Capacity; i++)
				{
					int index = i + receiverArray.Capacity;

					((IIndexed)newReceiverContents[index]).Index = -1;
				}
				*/
			}
			else
			{
				int lastReceiverFreeItemIndex = receiverArray.Count;

				for (int i = 0; i < donorArray.Count; i++)
				{
					int newIndex = i + receiverArray.Capacity;

					((IIndexed)newReceiverContents[lastReceiverFreeItemIndex]).Index = -1;

					((IIndexed)newReceiverContents[newIndex]).Index = lastReceiverFreeItemIndex;


					var swap = newReceiverContents[newIndex];

					newReceiverContents[newIndex] = newReceiverContents[lastReceiverFreeItemIndex];

					newReceiverContents[lastReceiverFreeItemIndex] = swap;


					lastReceiverFreeItemIndex++;
				}

				/*
				for (int i = donorArray.Count; i < donorArray.Capacity; i++)
				{
					int index = i + receiverArray.Capacity;

					((IIndexed)newReceiverContents[index]).Index = -1;
				}
				*/
			}

			((IContentsModifiable<IPoolElement<T>[]>)receiverArray).UpdateContents(newReceiverContents);

			((ICountModifiable)receiverArray).UpdateCount(receiverArray.Count + donorArray.Count);

			#endregion

			#region Update donor contents

			int newDonorCapacity = -1;

			switch (donorAllocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					newDonorCapacity = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					newDonorCapacity = donorAllocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID DONOR ALLOCATION COMMAND RULE: {donorAllocationCommand.Descriptor.Rule.ToString()}");
			}

			IPoolElement<T>[] newDonorContents = new IPoolElement<T>[newDonorCapacity];

			for (int i = 0; i < newDonorCapacity; i++)
				newDonorContents[i] = donorAllocationCommand.AllocationDelegate();

			((IContentsModifiable<IPoolElement<T>[]>)donorArray).UpdateContents(newDonorContents);

			((ICountModifiable)donorArray).UpdateCount(0);

			#endregion
		}

		#endregion

		#region Pool element allocation command

		public static AllocationCommand<IPoolElement<T>> BuildPoolElementAllocationCommand<T>(
			AllocationCommandDescriptor descriptor,
			Func<T> valueAllocationDelegate,
			Func<Func<T>, IPoolElement<T>> containerAllocationDelegate)
		{
			Func<IPoolElement<T>> poolElementAllocationDelegate = () =>
				containerAllocationDelegate(valueAllocationDelegate);

			var poolElementAllocationCommand = new AllocationCommand<IPoolElement<T>>
			{
				Descriptor = descriptor,

				AllocationDelegate = poolElementAllocationDelegate
			};

			return poolElementAllocationCommand;
		}

		#endregion

		#region Indexed container

		public static IndexedContainer<T> BuildIndexedContainer<T>(
			Func<T> allocationDelegate)
		{
			IndexedContainer<T> result = new IndexedContainer<T>((allocationDelegate != null)
				? allocationDelegate.Invoke()
				: default(T));

			return result;
		}

		#endregion

		#region Unmanaged array

		public unsafe static UnmanagedArray BuildUnmanagedArray(
			byte* memoryPointer,
			int elementSize,
			int elementCapacity = 0)
		{
			return new UnmanagedArray(
				memoryPointer,
				elementSize * elementCapacity,
				elementSize,
				elementCapacity);
		}

		public unsafe static UnmanagedArray BuildUnmanagedArrayGeneric<T>(
			byte* memoryPointer,
			int elementCapacity = 0)
		{
			int elementSize = Marshal.SizeOf(typeof(T));

			return new UnmanagedArray(
				memoryPointer,
				elementSize * elementCapacity,
				elementSize,
				elementCapacity);
		}

		public unsafe static void ResizeUnmanagedArray(
			ref UnmanagedArray array,
			byte* newMemoryPointer,
			int newElementCapacity)
		{
			Buffer.MemoryCopy(
				array.MemoryPointer,
				newMemoryPointer,
				newElementCapacity * array.ElementSize,
				array.ElementCapacity * array.ElementSize);

			array.ElementCapacity = newElementCapacity;
		}

		#endregion
	}
}