namespace HereticalSolutions.Collections
{
	/// <summary>
	/// Indicates that a collection contains another collection and allows access to it
	/// </summary>
	public interface IModifiable<TCollection>
	{
		/// <summary>
		/// Provides access to the nested collection
		/// </summary>
		TCollection Contents { get; }
		
		/// <summary>
		/// Allows to replace the nested collection with another
		/// </summary>
		/// <param name="newContents">The new collection</param>
		void UpdateContents(TCollection newContents);
	}
}