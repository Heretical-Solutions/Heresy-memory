using System;

namespace HereticalSolutions.Allocations
{
	public class AllocationCommand<T>
	{
		public AllocationCommandDescriptor Descriptor;

		public Func<T> AllocationDelegate;
	}
}