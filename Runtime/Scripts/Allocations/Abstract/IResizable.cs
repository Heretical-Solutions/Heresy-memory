namespace HereticalSolutions.Allocations.Internal
{
	public interface IResizable<T>
	{
		AllocationCommand<T> AllocationCommand { get; }
	}
}