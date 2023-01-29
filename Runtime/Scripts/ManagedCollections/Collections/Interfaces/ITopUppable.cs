using HereticalSolutions.Collections;

namespace HereticalSolutions.Allocations
{
	public interface ITopUppable<T>
	{
		void TopUp(IPoolElement<T> value);
	}
}