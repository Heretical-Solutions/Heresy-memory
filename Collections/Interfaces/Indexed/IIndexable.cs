namespace HereticalSolutions.Collections
{
	/// <summary>
	/// Indicates that the collection has a count of currently used elements and can access them by index
	/// </summary>
	public interface IIndexable<T>
	{
		/// <summary>
		/// The amount of currently used elements of the collection
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Returns the collection element by index
		/// </summary>
		T this[int index] { get; }

		/// <summary>
		/// Returns the collection element by integer index, but does not perform index validation
		/// </summary>
		/// <param name="index">The index</param>
		/// <returns>The collection element by integer index</returns>
		T Get(int index);
	}
}