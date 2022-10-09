namespace HereticalSolutions.Allocations.Internal
{
	public interface IContentsRetrievable<TCollection>
	{
		TCollection Contents { get; }
	}
}