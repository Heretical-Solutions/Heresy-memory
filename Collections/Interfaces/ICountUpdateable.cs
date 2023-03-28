namespace HereticalSolutions.Collections
{
    /// <summary>
    /// Indicates that the collection can be resized to the specified size on request
    /// </summary>
    public interface ICountUpdateable
    {
        /// <summary>
        /// Changes the size of the collection by allocating new or destroying old elements
        /// </summary>
        /// <param name="newCount">The new count</param>
        void UpdateCount(int newCount);
    }
}