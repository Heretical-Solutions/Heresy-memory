using System;

namespace HereticalSolutions.Allocations
{
	[Serializable]
	public struct AllocationCommandDescriptor
	{
		public EAllocationAmountRule Rule;
		
		public int Amount;
	}
}