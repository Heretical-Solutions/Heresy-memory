using System;
using HereticalSolutions.Allocations.Internal;

namespace HereticalSolutions.Collections.Managed
{
    /// <summary>
    /// The container that combines the functions of a memory pool and a list with an increased performance
    /// Basic concept is:
    /// 1. The contents are pre-allocated
    /// 2. Popping a new item is actually retrieving the first unused item and increasing the last used item index
    /// 3. Pushing an item is taking the last used item, swapping it with the removed item and decreasing the last used item index
    /// </summary>
    /// <typeparam name="T">Type of the objects stored in the container</typeparam>
    public class IndexedPackedArray<T>
        : IContentsRetrievable<IndexedContainer<T>[]>,
		  IContentsModifiable<IndexedContainer<T>[]>
    {
        private IndexedContainer<T>[] contents;

        public IndexedContainer<T>[] Contents { get { return contents; } }

        private int count;

        public IndexedPackedArray(IndexedContainer<T>[] contents)
        {
            this.contents = contents;
            
            count = 0;
        }

        public int Count { get { return count; } }

        public int Capacity { get { return contents.Length; } }

		public bool HasFreeSpace { get { return count < contents.Length; } }

        public void UpdateContents(IndexedContainer<T>[] newContents)
        {
            contents = newContents;
        }

		public IndexedContainer<T> this[int index]
		{
			get
			{
                if (index >= count || index < 0)
					throw new Exception(
                        string.Format(
							"[IndexedPackedArray<{0}>] INVALID INDEX: {1} COUNT:{2} CAPACITY:{3}",
                            typeof(T).ToString(),
                            index,
                            Count,
                            Capacity));

				return contents[index];
			}
		}

        public IndexedContainer<T> Get(int index)
        {
			if (index >= count || index < 0)
				throw new Exception(
					string.Format(
						"[IndexedPackedArray<{0}>] INVALID INDEX: {1} COUNT:{2} CAPACITY:{3}",
						typeof(T).ToString(),
						index,
						Count,
						Capacity));

            return contents[index];
        }

        /*
        public TChild Get<TChild>(int index) where TChild : T
        {
            if (index >= count || index < 0)
				throw new Exception(
					string.Format(
						"[IndexedPackedArray<{0}>] INVALID INDEX: {1} COUNT:{2} CAPACITY:{3}",
						typeof(T).ToString(),
						index,
						Count,
						Capacity));

            return (TChild)contents[index].Value;
        }
        */

        public IndexedContainer<T> Pop()
        {
            var result = contents[count];

            result.Index = count;

            count++;

            return result;
        }

        /*
        public TChild Pop<TChild>() where TChild : T
        {
            return (TChild)Pop();
        }
        */

        public void Push(IndexedContainer<T> item)
        {
            Push(item.Index);
        }

        public void Push(int index)
        {
            if (index >= count)
            {
                #if DEBUG_LOG
                Debug.Log("ATTEMPT TO DOUBLE PUSH ITEM");
                #endif

                return;
            }

            int lastItemIndex = count - 1;

            if (index != lastItemIndex)
            {
                contents[lastItemIndex].Index = index;

                contents[index].Index = -1;


                var swap = contents[index];

                contents[index] = contents[lastItemIndex];

                contents[lastItemIndex] = swap;
            }
            else
            {
				contents[index].Index = -1;
            }

            count--;
        }

    }
}