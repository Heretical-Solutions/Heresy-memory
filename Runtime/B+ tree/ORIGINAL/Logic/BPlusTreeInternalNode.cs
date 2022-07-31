using System;

namespace HereticalSolutions.Memory.Trees
{
    public static class BPlusTreeInternalNode
    {
        public static bool IsDeficient(BPlusTreeInternalNodeData data)
        {
            return data.degree < data.minDegree;
        }

        public static bool IsLendable(BPlusTreeInternalNodeData data)
        {
            return data.degree > data.minDegree;
        }

        public static bool IsMergeable(BPlusTreeInternalNodeData data)
        {
            return data.degree == data.minDegree;
        }

        public static bool IsOverfull(BPlusTreeInternalNodeData data)
        {
            return data.degree == data.maxDegree + 1;
        }
        
        public static void AppendChildPointer(
            BPlusTreeInternalNodeData data,
            BPlusTreeNodeData pointer)
        {
            data.childPointers[data.degree] = pointer;
            
            data.degree++;
        }

        public static int IndexOf(
            BPlusTreeInternalNodeData data,
            BPlusTreeNodeData pointer)
        {
            for (int i = 0; i < data.childPointers.Length; i++)
            {
                if (data.childPointers[i] == pointer)
                {
                    return i;
                }
            }
            
            return -1;
        }

        public static void InsertChildPointer(
            BPlusTreeInternalNodeData data,
            BPlusTreeNodeData pointer,
            int index)
        {
            for (int i = data.degree - 1; i >= index; i--)
            {
                data.childPointers[i + 1] = data.childPointers[i];
            }
            
            data.childPointers[index] = pointer;
            
            data.degree++;
        }

        public static void PrependChildPointer(
            BPlusTreeInternalNodeData data,
            BPlusTreeNodeData pointer)
        {
            for (int i = data.degree - 1; i >= 0; i--)
            {
                data.childPointers[i + 1] = data.childPointers[i];
            }
            
            data.childPointers[0] = pointer;
            
            data.degree++;
        }

        public static void RemoveKey(
            BPlusTreeInternalNodeData data,
            int index)
        {
            data.keys[index] = -1; //null;
        }

        public static void RemovePointer(
            BPlusTreeInternalNodeData data,
            int index)
        {
            data.childPointers[index] = null;
            
            data.degree--;
        }

        public static void RemovePointer(
            BPlusTreeInternalNodeData data,
            BPlusTreeNodeData pointer)
        {
            for (int i = 0; i < data.childPointers.Length; i++)
            {
                if (data.childPointers[i] == pointer)
                {
                    data.childPointers[i] = null;
                }
            }
            
            data.degree--;
        }
        
        public static int LinearNullSearch(BPlusTreeNodeData[] pointers)
        {
            for (int i = 0; i < pointers.Length; i++)
            {
                if (pointers[i] == null)
                {
                    return i;
                }
            }
            
            return -1;
        }
    }
}