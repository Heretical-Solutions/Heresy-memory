using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Unmanaged;
using System.Runtime.InteropServices;
using System;

namespace HereticalSolutions.Collections.Factories
{
	public static class CollectionFactory
	{
		#region Indexed packed array

		public static IndexedPackedArray<T> BuildIndexedPackedArray<T>(
			int capacity = 0)
		{
			IndexedContainer<T>[] contents = new IndexedContainer<T>[capacity];

			for (int i = 0; i < capacity; i++)
				contents[i] = new IndexedContainer<T>();

			return new IndexedPackedArray<T>(contents);
		}

		public static void ResizeIndexedPackedArray<T>(
			IndexedPackedArray<T> array,
			int newCapacity = -1)
		{
			if (newCapacity == -1)
				newCapacity = array.Capacity * 2;

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
					newContents[i] = new IndexedContainer<T>();
			}

			array.UpdateContents(newContents);
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