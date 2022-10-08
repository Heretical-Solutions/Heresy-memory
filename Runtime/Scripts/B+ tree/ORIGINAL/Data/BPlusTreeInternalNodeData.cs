using System;

namespace HereticalSolutions.Collections.Trees.UNFINISHED
{
    public class BPlusTreeInternalNodeData : BPlusTreeNodeData
    {
        public int maxDegree;
        
        public int minDegree;
        
        public int degree;
        
        public BPlusTreeInternalNodeData leftSibling;
        
        public BPlusTreeInternalNodeData rightSibling;
        
        public int[] keys;
        
        public BPlusTreeNodeData[] childPointers;
        
        public BPlusTreeInternalNodeData(
            int m,
            int[] keys,
            BPlusTreeNodeData[] pointers)
        {
            this.maxDegree = m;
            
            this.minDegree = (int) Math.Ceiling(m / 2.0);
            
            this.degree = BPlusTreeInternalNode.LinearNullSearch(pointers);
            
            this.keys = keys;
            
            this.childPointers = pointers;
        }
        
        public BPlusTreeInternalNodeData(
            int m,
            int[] keys)
        {
            this.maxDegree = m;
            
            this.minDegree = (int) Math.Ceiling(m / 2.0);
            
            this.degree = 0;
            
            this.keys = keys;
            
            this.childPointers = new BPlusTreeNodeData[this.maxDegree + 1];
        }
    }
}