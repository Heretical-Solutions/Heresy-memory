namespace HereticalSolutions.Allocations.Internal
{
	public interface IResizable<T>
	{
		AllocationCommand<T> ResizeAllocationCommand { get; }

		void Resize();
	}
}