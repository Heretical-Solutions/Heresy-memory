using System;

namespace HereticalSolutions.Collections.Allocations
{
	/// <summary>
	/// The command pattern that encapsulates an allocation descriptor and a delegate that produces a new instance of the desired type
	/// </summary>
	public class AllocationCommand<T>
	{
		/// <summary>
		/// The allocation descriptor
		/// </summary>
		public AllocationCommandDescriptor Descriptor;

		/// <summary>
		/// The allocation delegate
		/// </summary>
		public Func<T> AllocationDelegate;
	}
}