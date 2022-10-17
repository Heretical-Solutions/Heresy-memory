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

			var allocationCommand = pool.AllocationCommand;

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

		#region Indexed packed array pool

		public static IndexedPackedArrayPool<T> BuildIndexedPackedArrayPool<T>(
			AllocationCommand<IPoolElement<T>> initialAllocationCommand,
			AllocationCommand<IPoolElement<T>> additionalAllocationCommand)
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
			AllocationCommand<IPoolElement<T>> initialAllocationCommand)
		{
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

			IPoolElement<T>[] contents = new IPoolElement<T>[initialAmount];

			for (int i = 0; i < initialAmount; i++)
				contents[i] = initialAllocationCommand.AllocationDelegate();

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
					throw new Exception($"[CollectionFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR INDEXED PACKED ARRAY: {allocationCommand.Descriptor.Rule.ToString()}");
			}

			IPoolElement<T>[] newContents = new IPoolElement<T>[newCapacity];

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
					newContents[i] = allocationCommand.AllocationDelegate();
			}

			((IContentsModifiable<IPoolElement<T>[]>)array).UpdateContents(newContents);
		}

		#endregion

		#region Pool element allocation command

		public static AllocationCommand<IPoolElement<T>> BuildPoolElementAllocationCommand<T>(
			//AllocationCommand<T> valueAllocationCommand,
			AllocationCommandDescriptor descriptor,
			Func<T> valueAllocationDelegate,
			Func<Func<T>, IPoolElement<T>> containerAllocationDelegate)
		{
			Func<IPoolElement<T>> poolElementAllocationDelegate = () =>
				containerAllocationDelegate(
					valueAllocationDelegate); //valueAllocationCommand.AllocationDelegate);

			var poolElementAllocationCommand = new AllocationCommand<IPoolElement<T>>
			{
				Descriptor = descriptor, //valueAllocationCommand.Descriptor,

				AllocationDelegate = poolElementAllocationDelegate
			};

			return poolElementAllocationCommand;
		}

		#endregion

		#region Indexed container

		public static IndexedContainer<T> BuildIndexedContainer<T>(
			Func<T> allocationDelegate)
		{
			IndexedContainer<T> result = new IndexedContainer<T>(allocationDelegate());

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