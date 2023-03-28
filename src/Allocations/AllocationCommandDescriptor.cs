using System;

namespace HereticalSolutions.Collections.Allocations
{
	/// <summary>
	/// Encapsulates the allocation rule and the amount of allocated elements
	/// </summary>
	[Serializable]
	public struct AllocationCommandDescriptor
	{
		/// <summary>
		/// The rule
		/// </summary>
		public EAllocationAmountRule Rule;
		
		/// <summary>
		/// The amount
		/// </summary>
		public int Amount;
	}
}