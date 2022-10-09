namespace HereticalSolutions.Collections.Managed
{
	public class IndexedContainer<T> : IPoolElement<T>
	{
		public int Index = -1;

		public T Value { get; set; } = default(T);

		public IndexedContainer(
			T initialValue)
		{
			Value = initialValue;
		}
	}
}