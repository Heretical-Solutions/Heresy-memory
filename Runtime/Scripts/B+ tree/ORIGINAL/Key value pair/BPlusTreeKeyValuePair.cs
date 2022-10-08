using System;
using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Trees.UNFINISHED
{
    public class BPlusTreeKeyValuePair :
        IComparable<BPlusTreeKeyValuePair>
    {
        public int key;
        
        public double value; //TODO: replace with generic or smth

        public BPlusTreeKeyValuePair(
            int key,
            double value)
        {
            this.key = key;
            
            this.value = value;
        }

        public int CompareTo(BPlusTreeKeyValuePair other)
        {
            if (key == other.key)
            {
                return 0;
            }
            
            if (key > other.key)
            {
                return 1;
            }
            
            return -1;
        }

        /*
        //TODO: replace
        public int compareTo(BPlusTreeKeyValuePair o)
        {
            if (key == o.key)
            {
                return 0;
            }
            else if (key > o.key)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        */
    }
}