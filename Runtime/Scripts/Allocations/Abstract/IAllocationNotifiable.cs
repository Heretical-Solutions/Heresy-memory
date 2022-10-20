using HereticalSolutions.Collections;

namespace HereticalSolutions.Allocations
{
	public interface IAllocationNotifiable<T>
	{
		void Notify(IPoolElement<T> element);
	}
}