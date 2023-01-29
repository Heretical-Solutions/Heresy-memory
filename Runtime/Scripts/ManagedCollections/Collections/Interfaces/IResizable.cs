namespace HereticalSolutions.Allocations
{
	public interface IResizable<T>
	{
		AllocationCommand<T> ResizeAllocationCommand { get; }

		void Resize();
	}
}