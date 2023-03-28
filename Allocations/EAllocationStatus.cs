namespace HereticalSolutions.Collections.Allocations
{
    /// <summary>
    /// Specifies the status of the allocation
    /// </summary>
    public enum EAllocationStatus
    {
        /// <summary>
        /// The element is free for use
        /// </summary>
        FREE,
        
        /// <summary>
        /// The element is in use
        /// </summary>
        USED
    }
}