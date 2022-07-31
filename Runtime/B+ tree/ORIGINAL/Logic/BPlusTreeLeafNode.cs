using System;

namespace HereticalSolutions.Memory.Trees
{
    public static class BPlusTreeLeafNode
    {
        public static bool IsDeficient(BPlusTreeLeafNodeData data)
        {
            return data.numPairs < data.minNumPairs;
        }

        public static bool IsFull(BPlusTreeLeafNodeData data)
        {
            return data.numPairs == data.maxNumPairs;
        }

        public static bool IsLendable(BPlusTreeLeafNodeData data)
        {
            return data.numPairs > data.minNumPairs;
        }

        public static bool IsMergeable(BPlusTreeLeafNodeData data)
        {
            return data.numPairs == data.minNumPairs;
        }

        public static void Insert(
            BPlusTreeLeafNodeData data,
            BPlusTreeKeyValuePair keyValuePair)
        {
            data.dictionary[data.numPairs] = keyValuePair;
                
            data.numPairs++;
                
            //TODO: replace
            Array.Sort(
                data.dictionary,
                0,
                data.numPairs);
        }

        public static bool TryInsert(
            BPlusTreeLeafNodeData data,
            BPlusTreeKeyValuePair keyValuePair)
        {
            if (IsFull(data))
            {
                return false;
            }
            else
            {
                Insert(
                    data,
                    keyValuePair);

                return true;
            }
        }
        
        public static void Delete(
            BPlusTreeLeafNodeData data,
            int index)
        {
            data.dictionary[index] = null;
            
            data.numPairs--;
        }
        
        public static int LinearNullSearch(BPlusTreeKeyValuePair[] dictionary)
        {
            for (int i = 0; i < dictionary.Length; i++)
            {
                if (dictionary[i] == null)
                {
                    return i;
                }
            }
            
            return -1;
        }
    }
}