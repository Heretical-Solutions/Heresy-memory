using System.Collections.Generic;

namespace HereticalSolutions.Collections.Trees.UNFINISHED
{
    public class BPlusTreeData
    {
        public int m;
        
        public BPlusTreeInternalNodeData root;
        
        public BPlusTreeLeafNodeData firstLeaf;
        
        public BPlusTreeData(int m)
        {
            this.m = m;
            
            this.root = null;
        }
    }
}