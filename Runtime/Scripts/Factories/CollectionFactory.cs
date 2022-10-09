using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Internal;

using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Unmanaged;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Collections.Factories
{
	public static class CollectionFactory
	{
		#region Stack pool

		public static StackPool<T> BuildStackPool<T>(
					AllocationCommand<T> initialAllocationCommand,
					AllocationCommand<T> additionalAllocationCommand)
		{
			var stack = new Stack<T>();

			int initialAmount = -1;

			switch (initialAllocationCommand.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = initialAllocationCommand.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Rule.ToString()}");
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

			var allocationCommand = pool.AllocationCommand;

			int addedCapacity = -1;

			switch (allocationCommand.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					addedCapacity = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					addedCapacity = allocationCommand.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Rule.ToString()}");
			}

			for (int i = 0; i < addedCapacity; i++)
				stack.Push(
					allocationCommand.AllocationDelegate());
		}

		#endregion

		#region Indexed packed array pool

		public static IndexedPackedArrayPool<T> BuildIndexedPackedArrayPool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand)
		{
			var pool = BuildIndexedPackedArray<T>(initialAllocationCommand);

			return new IndexedPackedArrayPool<T>(
				pool,
				ResizeIndexedPackedArrayPool,
				additionalAllocationCommand);
		}

		public static void ResizeIndexedPackedArrayPool<T>(
			IndexedPackedArrayPool<T> pool)
		{
			ResizeIndexedPackedArray(
				((IContentsRetrievable<IndexedPackedArray<T>>)pool).Contents,
				pool.AllocationCommand);
		}

		#endregion

		#region Indexed packed array

		public static IndexedPackedArray<T> BuildIndexedPackedArray<T>(
			AllocationCommand<T> initialAllocationCommand)
		{
			int initialAmount = -1;

			switch (initialAllocationCommand.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = initialAllocationCommand.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Rule.ToString()}");
			}

			IndexedContainer<T>[] contents = new IndexedContainer<T>[initialAmount];

			for (int i = 0; i < initialAmount; i++)
				contents[i] = new IndexedContainer<T>(
					initialAllocationCommand.AllocationDelegate());

			return new IndexedPackedArray<T>(contents);
		}

		public static void ResizeIndexedPackedArray<T>(
			IndexedPackedArray<T> array,
			AllocationCommand<T> allocationCommand)
		{
			int newCapacity = -1;

			switch (allocationCommand.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					newCapacity = array.Capacity + 1;
					break;

				case EAllocationAmountRule.DOUBLE_AMOUNT:
					newCapacity = Math.Max(array.Capacity, 1) * 2;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					newCapacity = array.Capacity + allocationCommand.Amount;
					break;

				default:
					throw new Exception($"[CollectionFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR INDEXED PACKED ARRAY: {allocationCommand.Rule.ToString()}");
			}

			IndexedContainer<T>[] newContents = new IndexedContainer<T>[newCapacity];

			if (newCapacity <= array.Capacity)
			{
				for (int i = 0; i < newCapacity; i++)
					newContents[i] = array[i];
			}
			else
			{
				for (int i = 0; i < array.Capacity; i++)
					newContents[i] = array[i];

				for (int i = array.Capacity; i < newCapacity; i++)
					newContents[i] = new IndexedContainer<T>(
						allocationCommand.AllocationDelegate());
			}

			((IContentsModifiable<IndexedContainer<T>[]>)array).UpdateContents(newContents);
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