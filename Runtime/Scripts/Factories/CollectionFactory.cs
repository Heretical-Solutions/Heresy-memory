using HereticalSolutions.Collections.Unmanaged;
using System.Runtime.InteropServices;
using System;

namespace HereticalSolutions.Collections.Factories
{
	public static class CollectionFactory
	{
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