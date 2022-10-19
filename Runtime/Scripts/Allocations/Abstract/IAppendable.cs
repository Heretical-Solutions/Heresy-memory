using HereticalSolutions.Collections;

namespace HereticalSolutions.Allocations.Internal
{
	public interface IAppendable<T>
	{
		AllocationCommand<T> AppendAllocationCommand { get; }

		T Append();
	}
}