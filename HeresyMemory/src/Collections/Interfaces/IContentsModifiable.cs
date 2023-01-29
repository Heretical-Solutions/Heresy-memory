namespace HereticalSolutions.Collections
{
	public interface IContentsModifiable<TCollection>
	{
		void UpdateContents(TCollection newContents);
	}
}