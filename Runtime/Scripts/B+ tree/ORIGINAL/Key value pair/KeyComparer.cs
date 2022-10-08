using System.Collections.Generic;

namespace HereticalSolutions.Collections.Trees.UNFINISHED
{
    public class KeyComparer : IComparer<BPlusTreeKeyValuePair>
    {
        public int Compare(BPlusTreeKeyValuePair a, BPlusTreeKeyValuePair b)
        {
            return a.key.CompareTo(b.key);
        }
    }
}