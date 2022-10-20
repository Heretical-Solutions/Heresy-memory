using HereticalSolutions.Collections;

namespace HereticalSolutions.Allocations
{
	public interface IAppendable<T>
	{
		AllocationCommand<T> AppendAllocationCommand { get; }

		T Append();
	}
}