namespace HereticalSolutions.Allocations
{
	public interface IContentsModifiable<TCollection>
	{
		void UpdateContents(TCollection newContents);
	}
}