using System;

namespace HereticalSolutions.Allocations
{
	public class AllocationCommand<T>
	{
		public EAllocationAmountRule Rule;

		public Func<T> AllocationDelegate;

		public int Amount;
	}
}