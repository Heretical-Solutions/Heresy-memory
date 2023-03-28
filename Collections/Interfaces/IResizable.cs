using HereticalSolutions.Collections.Allocations;

namespace HereticalSolutions.Collections
{
	/// <summary>
	/// Indicates that the collection can change its size
	/// </summary>
	public interface IResizable<T>
	{
		/// <summary>
		/// The command for resizing allocations
		/// </summary>
		AllocationCommand<T> ResizeAllocationCommand { get; }

		/// <summary>
		/// Adds new elements to the collection
		/// </summary>
		void Resize();
	}
}