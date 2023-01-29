namespace HereticalSolutions.Collections
{
	public interface IContentsRetrievable<TCollection>
	{
		TCollection Contents { get; }
	}
}