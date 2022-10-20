namespace HereticalSolutions.Allocations
{
	public interface IContentsRetrievable<TCollection>
	{
		TCollection Contents { get; }
	}
}