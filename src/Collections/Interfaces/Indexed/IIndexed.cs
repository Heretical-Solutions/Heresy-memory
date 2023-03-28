namespace HereticalSolutions.Collections
{
	/// <summary>
	/// Indicates that the element has an integer index in the collection it belongs to
	/// </summary>
	public interface IIndexed
	{
		/// <summary>
		/// The index of the element
		/// </summary>
		int Index { get; set; }
	}
}