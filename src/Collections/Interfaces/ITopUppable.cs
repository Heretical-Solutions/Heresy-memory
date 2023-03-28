namespace HereticalSolutions.Collections
{
	/// <summary>
	/// Indicates that the collection can fill the element with new contents on request
	/// </summary>
	public interface ITopUppable<T>
	{
		/// <summary>
		/// Fills the collection element with new contents
		/// </summary>
		/// <param name="value">New contents</param>
		void TopUp(T value);
	}
}