using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools
{
	public interface IDecoratedPool<T>
	{
		T Pop(IPoolDecoratorArgument[] args);

		void Push(T instance);
	}
}