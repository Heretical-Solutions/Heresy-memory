namespace HereticalSolutions.Collections.Allocations
{
	/// <summary>
	/// Specifies the rule by which the amount of elements in the collection is calculated during initialization or resizing
	/// </summary>
	public enum EAllocationAmountRule
	{
		/// <summary>
		/// Zero elements are allocated
		/// </summary>
		ZERO,
		
		/// <summary>
		/// One new element is added
		/// </summary>
		ADD_ONE,
		
		/// <summary>
		/// The amount is doubled,
		/// </summary>
		DOUBLE_AMOUNT,
		
		/// <summary>
		/// The specified amount is added
		/// </summary>
		ADD_PREDEFINED_AMOUNT
	}
}