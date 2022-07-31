using System.Collections.Generic;

namespace HereticalSolutions.Memory.Trees
{
    public class KeyComparer : IComparer<BPlusTreeKeyValuePair>
    {
        public int Compare(BPlusTreeKeyValuePair a, BPlusTreeKeyValuePair b)
        {
            return a.key.CompareTo(b.key);
        }
    }
}