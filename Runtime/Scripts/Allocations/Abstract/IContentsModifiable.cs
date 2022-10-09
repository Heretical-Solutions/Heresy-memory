namespace HereticalSolutions.Allocations.Internal
{
	public interface IContentsModifiable<TCollection>
	{
		void UpdateContents(TCollection newContents);
	}
}