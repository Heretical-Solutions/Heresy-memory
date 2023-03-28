using HereticalSolutions.Collections.Allocations;

namespace HereticalSolutions.Collections
{
	/// <summary>
	/// Indicates that the collection can be appended with a new element on request
	/// </summary>
	public interface IAppendable<T>
	{
		/// <summary>
		/// The command for append allocations
		/// </summary>
		AllocationCommand<T> AppendAllocationCommand { get; }

		/// <summary>
		/// Allocates a new element
		/// </summary>
		/// <returns>The new element</returns>
		T Append();
	}
}