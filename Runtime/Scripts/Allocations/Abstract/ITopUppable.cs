using System;
using HereticalSolutions.Collections;

namespace HereticalSolutions.Allocations.Internal
{
	public interface ITopUppable<T>
	{
		Func<T> TopUpAllocationDelegate { get; }

		void TopUp(IPoolElement<T> value);
	}
}