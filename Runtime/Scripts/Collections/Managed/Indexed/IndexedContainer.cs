namespace HereticalSolutions.Collections.Managed
{
	public class IndexedContainer<T> 
		: IPoolElement<T>,
		IIndexable
	{
		public int Index { get; set; }

		public T Value { get; set; } = default(T);

		public IndexedContainer(
			T initialValue)
		{
			Value = initialValue;

			Index = -1;
		}
	}
}