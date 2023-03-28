namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Indicates that the collection has a fixed size (like an array)
    /// </summary>
    public interface IFixedSizeCollection<T>
    {
        /// <summary>
        /// The current maximum size of the collection
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Retrieves an element of the collection by index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The collection element by index</returns>
        T ElementAt(int index);
    }
}