using System;

namespace HereticalSolutions.Collections.Trees.UNFINISHED
{
    public class BPlusTreeLeafNodeData : BPlusTreeNodeData
    {
        public int maxNumPairs;
        
        public int minNumPairs;
        
        public int numPairs;
        
        public BPlusTreeLeafNodeData leftSibling;
        
        public BPlusTreeLeafNodeData rightSibling;
        
        
        public BPlusTreeKeyValuePair[] dictionary;
        
        public BPlusTreeLeafNodeData(
            int m,
            BPlusTreeKeyValuePair keyValuePair)
        {
            this.maxNumPairs = m - 1;
            
            this.minNumPairs = (int) (Math.Ceiling((float)m / 2f) - 1);
            
            this.dictionary = new BPlusTreeKeyValuePair[m];
            
            this.numPairs = 0;
            
            BPlusTreeLeafNode.Insert(
                this,
                keyValuePair);
        }

        public BPlusTreeLeafNodeData(
            int m,
            BPlusTreeKeyValuePair[] dictionary,
            BPlusTreeInternalNodeData parent)
        {
            this.maxNumPairs = m - 1;
            
            this.minNumPairs = (int) (Math.Ceiling((float)m / 2f) - 1);
            
            this.dictionary = dictionary;
            
            this.numPairs = BPlusTreeLeafNode.LinearNullSearch(dictionary);
            
            this.parent = parent;
        }
    }
}