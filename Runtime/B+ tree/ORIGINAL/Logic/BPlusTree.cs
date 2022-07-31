using System;
using System.Collections.Generic;

namespace HereticalSolutions.Memory.Trees
{
    public static class BPlusTree
    {
        private static KeyComparer keyComparer = new KeyComparer();
        
        #region Insert

        /// <summary>
        /// Insert value by the given key into tree
        /// </summary>
        /// <param name="data">tree</param>
        /// <param name="key">integer lookup key</param>
        /// <param name="value">data to be inserted</param>
        public static void Insert(
            BPlusTreeData data,
            int key,
            double value)
        {
            var keyValuePair = new BPlusTreeKeyValuePair(
                key,
                value);
            
            //If there is no first leaf node in the tree then we create one and insert the key-value pair as its content
            if (IsEmpty(data))
            {
                CreateAndInsertFirstLeafNode(
                    data,
                    keyValuePair);
                
                return;
            }
            
            //Since every element is inserted into the leaf node, go to the appropriate leaf node
            BPlusTreeLeafNodeData leafNode = (data.root == null)
                ? data.firstLeaf
                : FindLeafNode(
                    data,
                    key);

            //If the leaf is not full, insert the key into the leaf node in increasing order
            if (BPlusTreeLeafNode.TryInsert(
                leafNode,
                keyValuePair))
                return;
            
            //If the leaf is full, insert the key into the leaf node in increasing order and balance the tree in the following way.
            BPlusTreeLeafNode.Insert(
                leafNode,
                keyValuePair);
            
            //Break the node at m/2th position.
            int midpoint = GetMidpoint(data.m);
                    
            BPlusTreeKeyValuePair[] halfDict = SplitDictionaryInHalf(
                leafNode,
                midpoint,
                data.m);

            //Add m/2th key to the parent node as well (if there is one)
            int medianKey = halfDict[0].key;
            
            if (leafNode.parent == null)
            {
                CreateParentNode(
                    leafNode,
                    medianKey,
                    data.m);
            }
            else
            {
                AddMedianKeyToParentNode(
                    leafNode,
                    medianKey);
            }
            
            SplitLeafNodeInHalf(
                leafNode,
                halfDict,
                data.m);

            //If the parent node is already full, follow steps 2 to 3.
            ValidateParentNode(
                data,
                leafNode);
        }
        
        /// <summary>
        /// Create and insert the first leaf node into tree if it has none
        /// </summary>
        /// <param name="data">tree</param>
        /// <param name="keyValuePair">key value pair</param>
        private static void CreateAndInsertFirstLeafNode(
            BPlusTreeData data,
            BPlusTreeKeyValuePair keyValuePair)
        {
            BPlusTreeLeafNodeData leafNode = new BPlusTreeLeafNodeData(
                data.m,
                keyValuePair);

            data.firstLeaf = leafNode;
        }

        /// <summary>
        /// Split the dictionary into two pieces starting with splitting point
        /// </summary>
        /// <param name="leafNode">B+ tree leaf node</param>
        /// <param name="split">splitting point</param>
        /// <param name="m">m value</param>
        /// <returns></returns>
        private static BPlusTreeKeyValuePair[] SplitDictionaryInHalf(
            BPlusTreeLeafNodeData leafNode,
            int split,
            int m)
        {

            BPlusTreeKeyValuePair[] dictionary = leafNode.dictionary;

            BPlusTreeKeyValuePair[] halfDict = new BPlusTreeKeyValuePair[m];

            for (int i = split; i < dictionary.Length; i++)
            {
                halfDict[i - split] = dictionary[i];
                
                BPlusTreeLeafNode.Delete(
                    leafNode,
                    i);
            }

            return halfDict;
        }

        /// <summary>
        /// Create a parent internal node to leaf node if it doesn't have one
        /// </summary>
        /// <param name="leafNode">leaf node</param>
        /// <param name="medianKey">median key</param>
        /// <param name="m">m value</param>
        private static void CreateParentNode(
            BPlusTreeLeafNodeData leafNode,
            int medianKey,
            int m)
        {
            int[] parentKeys = new int[m];
                        
            parentKeys[0] = medianKey;
                        
            BPlusTreeInternalNodeData parent = new BPlusTreeInternalNodeData(
                m,
                parentKeys);
                        
            leafNode.parent = parent;
                        
            BPlusTreeInternalNode.AppendChildPointer(
                parent,
                leafNode);
        }
        
        /// <summary>
        /// Add median key to parent internal node keys
        /// </summary>
        /// <param name="leafNode">leaf node</param>
        /// <param name="medianKey">median key</param>
        private static void AddMedianKeyToParentNode(
            BPlusTreeLeafNodeData leafNode,
            int medianKey)
        {    
            leafNode.parent.keys[leafNode.parent.degree - 1] = medianKey;
                
            Array.Sort(
                leafNode.parent.keys,
                0,
                leafNode.parent.degree);
        }
        
        /// <summary>
        /// Split the leaf node in two and add new leaf node to parent node's children
        /// </summary>
        /// <param name="sourceLeafNode">source leaf node</param>
        /// <param name="halfDict">half of the node's keys that go to new leaf node</param>
        /// <param name="m">m value</param>
        private static void SplitLeafNodeInHalf(
            BPlusTreeLeafNodeData sourceLeafNode,
            BPlusTreeKeyValuePair[] halfDict,
            int m)
        {
            BPlusTreeLeafNodeData newLeafNode = new BPlusTreeLeafNodeData(
                m,
                halfDict,
                sourceLeafNode.parent);

            int targetIndex = BPlusTreeInternalNode
                .IndexOf(
                    sourceLeafNode.parent,
                    sourceLeafNode)
                + 1;
                    
            BPlusTreeInternalNode.InsertChildPointer(
                sourceLeafNode.parent,
                newLeafNode,
                targetIndex);

            MakeNewRightSibling(
                sourceLeafNode,
                newLeafNode);
        }
        
        #endregion

        #region Search

        public static double Search(
            BPlusTreeData data,
            int key)
        {
            if (IsEmpty(data))
            {
                return default(double); //null;
            }

            BPlusTreeLeafNodeData leafNode = (data.root == null)
                ? data.firstLeaf
                : FindLeafNode(
                    data,
                    key);

            BPlusTreeKeyValuePair[] dictionary = leafNode.dictionary;
            
            int index = BinarySearch(
                data,
                dictionary,
                leafNode.numPairs,
                key);

            if (index < 0)
            {
                return default(double); //null;
            }
            else
            {
                return dictionary[index].value;
            }
        }

        private static int BinarySearch(
            BPlusTreeData data,
            BPlusTreeKeyValuePair[] dictionary,
            int numPairs,
            int t)
        {
            return Array.BinarySearch(
                dictionary,
                0,
                numPairs,
                new BPlusTreeKeyValuePair(t, 0),
                keyComparer);
        }

        public static List<double> Search(
            BPlusTreeData data,
            int lowerBound,
            int upperBound)
        {
            List<double> values = new List<double>();

            BPlusTreeLeafNodeData currNode = data.firstLeaf;
            
            while (currNode != null)
            {
                BPlusTreeKeyValuePair[] dictionary = currNode.dictionary;
                
                foreach (BPlusTreeKeyValuePair keyValuePair in dictionary)
                {
                    if (keyValuePair == null)
                    {
                        break;
                    }

                    if (lowerBound <= keyValuePair.key
                        && keyValuePair.key <= upperBound)
                    {
                        values.Add(keyValuePair.value);
                    }
                }
                
                currNode = currNode.rightSibling;
            }

            return values;
        }

        #endregion

        #region Finding leaf nodes

        // Find the leaf node
        private static BPlusTreeLeafNodeData FindLeafNode(
            BPlusTreeData data,
            int key)
        {
            int[] keys = data.root.keys;
            
            int i;

            for (i = 0; i < data.root.degree - 1; i++)
            {
                if (key < keys[i])
                {
                    break;
                }
            }

            BPlusTreeNodeData child = data.root.childPointers[i];
            
            if (child is BPlusTreeLeafNodeData)
            {
                return (BPlusTreeLeafNodeData) child;
            } 
            else
            {
                return FindLeafNode(
                    (BPlusTreeInternalNodeData) child,
                    key);
            }
        }

        // Find the leaf node
        private static BPlusTreeLeafNodeData FindLeafNode(
            BPlusTreeInternalNodeData node,
            int key)
        {
            int[] keys = node.keys;
            
            int i;

            for (i = 0; i < node.degree - 1; i++)
            {
                if (key < keys[i])
                {
                    break;
                }
            }
            
            BPlusTreeNodeData childNode = node.childPointers[i];
            
            if (childNode is BPlusTreeLeafNodeData)
            {
                return (BPlusTreeLeafNodeData) childNode;
            }
            else
            {
                return FindLeafNode(
                    (BPlusTreeInternalNodeData)node.childPointers[i],
                    key);
            }
        }

        #endregion

        public static bool IsEmpty(BPlusTreeData data)
        {
            return data.firstLeaf == null;
        }

        // Finding the index of the pointer
        private static int FindIndexOfPointer(
            BPlusTreeNodeData[] pointers,
            BPlusTreeLeafNodeData node)
        {
            int i;
            
            for (i = 0; i < pointers.Length; i++)
            {
                if (pointers[i] == node)
                {
                    break;
                }
            }
            
            return i;
        }

        // Get the mid point
        private static int GetMidpoint(int m)
        {
            return (int) Math.Ceiling(((float)m + 1) / 2f) - 1;
        }
        
        #region Sibling reassignment
        
        private static void MakeNewRightSibling(
            BPlusTreeLeafNodeData leafNode,
            BPlusTreeLeafNodeData newSibling)
        {
            newSibling.rightSibling = leafNode.rightSibling;
                    
            if (newSibling.rightSibling != null)
            {
                newSibling.rightSibling.leftSibling = newSibling;
            }
                    
            leafNode.rightSibling = newSibling;
                    
            newSibling.leftSibling = leafNode;
        }
        
        private static void MakeNewRightSibling(
            BPlusTreeInternalNodeData internalNode,
            BPlusTreeInternalNodeData newSibling)
        {
            newSibling.rightSibling = internalNode.rightSibling;
            
            if (newSibling.rightSibling != null)
            {
                newSibling.rightSibling.leftSibling = newSibling;
            }
            
            internalNode.rightSibling = newSibling;
            
            newSibling.leftSibling = internalNode;
        }
        
        #endregion
        
        #region Validation
        
        private static void ValidateParentNode(
            BPlusTreeData data,
            BPlusTreeLeafNodeData leafNode)
        {
            if (data.root == null)
            {
                data.root = leafNode.parent;
                
                return;
            }
            
            BPlusTreeInternalNodeData internalNode = leafNode.parent;
                
            while (internalNode != null)
            {
                if (BPlusTreeInternalNode.IsOverfull(internalNode))
                {
                    SplitInternalNode(
                        data,
                        internalNode,
                        data.m);
                }
                else 
                {
                    break;
                }
                
                internalNode = internalNode.parent;
            }
        }
        
        private static void SplitInternalNode(
            BPlusTreeData data,
            BPlusTreeInternalNodeData internalNode,
            int m)
        {
            BPlusTreeInternalNodeData parent = internalNode.parent;

            //Find median point key that goes to parent node
            int midpoint = GetMidpoint(m);
            
            int newParentKey = internalNode.keys[midpoint];
            
            //Create sibling node that will take half the keys and pointers
            int[] halfKeys = SplitKeys(
                internalNode.keys,
                midpoint,
                m);
            
            BPlusTreeNodeData[] halfPointers = SplitChildPointers(
                internalNode,
                midpoint,
                m);

            internalNode.degree = BPlusTreeInternalNode.LinearNullSearch(internalNode.childPointers);

            BPlusTreeInternalNodeData newSibling = new BPlusTreeInternalNodeData(
                m,
                halfKeys,
                halfPointers);
            
            //Assign the new sibling as a parent node to half of the leaf nodes that gets detached
            foreach (BPlusTreeNodeData pointer in halfPointers)
            {
                if (pointer != null)
                {
                    pointer.parent = newSibling;
                }
            }

            //Reassign siblings for current node, its previous and new siblings
            MakeNewRightSibling(
                internalNode,
                newSibling);

            //Update parent node with new sibling as its new child
            if (parent == null)
            {
                var newSharedParentNode = CreateNewSharedParentNode(
                    internalNode,
                    newSibling,
                    newParentKey,
                    m);
                
                data.root = newSharedParentNode;
            }
            else
            {
                ShareParentWithNewNode(
                    parent,
                    internalNode,
                    newSibling,
                    newParentKey);
            }
        }
        
        /// <summary>
        /// Create new internal to act as a parent to both source and sibling nodes
        /// </summary>
        /// <param name="sourceNode">source node</param>
        /// <param name="siblingNode">sibling node</param>
        /// <param name="newParentKey">median point key that goes to parent node</param>
        /// <param name="m">m value</param>
        /// <returns>new parent node</returns>
        private static BPlusTreeInternalNodeData CreateNewSharedParentNode(
            BPlusTreeInternalNodeData sourceNode,
            BPlusTreeInternalNodeData siblingNode,
            int newParentKey,
            int m)
        {
            int[] keys = new int[m];
                
            keys[0] = newParentKey;
                
            BPlusTreeInternalNodeData newParentNode = new BPlusTreeInternalNodeData(
                m,
                keys);
                
            BPlusTreeInternalNode.AppendChildPointer(
                newParentNode,
                sourceNode);
                
            BPlusTreeInternalNode.AppendChildPointer(
                newParentNode,
                siblingNode);
            
            sourceNode.parent = newParentNode;
                
            siblingNode.parent = newParentNode;
            
            return newParentNode;
        }
        
        /// <summary>
        /// Assign sibling node as a child node to the source node's parent node
        /// </summary>
        /// <param name="parentNode">parent node</param>
        /// <param name="sourceNode">source node</param>
        /// <param name="siblingNode">sibling node</param>
        /// <param name="newParentKey">median point key that goes to parent node</param>
        private static void ShareParentWithNewNode(
            BPlusTreeInternalNodeData parentNode,
            BPlusTreeInternalNodeData sourceNode,
            BPlusTreeInternalNodeData siblingNode,
            int newParentKey
        )
        {
            parentNode.keys[parentNode.degree - 1] = newParentKey;
                
            Array.Sort(parentNode.keys, 0, parentNode.degree);

            int pointerIndex = BPlusTreeInternalNode
                .IndexOf(
                    parentNode,
                    sourceNode)
                + 1;
                
            BPlusTreeInternalNode.InsertChildPointer(
                parentNode,
                siblingNode,
                pointerIndex);
                
            siblingNode.parent = parentNode;
        }
        
        private static int[] SplitKeys(
            int[] keys,
            int split,
            int m)
        {
            int[] halfKeys = new int[m];

            keys[split] = -1; //null;

            for (int i = split + 1; i < keys.Length; i++)
            {
                halfKeys[i - split - 1] = keys[i];
                
                keys[i] = -1; //null;
            }

            return halfKeys;
        }
        
        private static BPlusTreeNodeData[] SplitChildPointers(
            BPlusTreeInternalNodeData internalNode,
            int split,
            int m)
        {
            BPlusTreeNodeData[] pointers = internalNode.childPointers;
            
            BPlusTreeNodeData[] halfPointers = new BPlusTreeNodeData[m + 1];

            for (int i = split + 1; i < pointers.Length; i++)
            {
                halfPointers[i - split - 1] = pointers[i];
                
                BPlusTreeInternalNode.RemovePointer(
                    internalNode,
                    i);
            }

            return halfPointers;
        }
        
        #endregion
        
        #region Balancing
        
        // Balance the tree
        public static void HandleDeficiency(
            BPlusTreeData data,
            BPlusTreeInternalNodeData internalNode)
        {
            BPlusTreeInternalNodeData sibling;
            
            BPlusTreeInternalNodeData parent = internalNode.parent;

            if (data.root == internalNode)
            {
                PromoteChildInternalNodeToRoot(
                    data,
                    internalNode);
            }
            else if (internalNode.leftSibling != null
                     && BPlusTreeInternalNode.IsLendable(internalNode.leftSibling))
            {
                sibling = internalNode.leftSibling; //it ain't doing much
            }
            else if (internalNode.rightSibling != null
                     && BPlusTreeInternalNode.IsLendable(internalNode.rightSibling))
            {
                LendRightSibling(
                    internalNode,
                    data.m);
            }
            else if (internalNode.leftSibling != null
                && BPlusTreeInternalNode.IsMergeable(internalNode.leftSibling))
            {

            }
            else if (internalNode.rightSibling != null
                && BPlusTreeInternalNode.IsMergeable(internalNode.rightSibling))
            {
                MergeWithRightSibling(internalNode);
            }

            if (parent != null
                && BPlusTreeInternalNode.IsDeficient(parent))
            {
                HandleDeficiency(
                    data,
                    parent);
            }
        }
        
        /// <summary>
        /// Find the first (last?) child internal node and make it a new root or if there is none then leave root empty
        /// </summary>
        /// <param name="data">tree</param>
        /// <param name="internalNode">internal node</param>
        private static void PromoteChildInternalNodeToRoot(
            BPlusTreeData data,
            BPlusTreeInternalNodeData internalNode)
        {
            for (int i = 0; i < internalNode.childPointers.Length; i++)
            {
                if (internalNode.childPointers[i] != null)
                {
                    if (internalNode.childPointers[i] is BPlusTreeInternalNodeData)
                    {
                        data.root = (BPlusTreeInternalNodeData)internalNode.childPointers[i];
                        
                        data.root.parent = null; //Shouldn't there be a break here?
                    }
                    else if (internalNode.childPointers[i] is BPlusTreeLeafNodeData)
                    {
                        data.root = null;
                    }
                }
            }
        }
        
        /// <summary>
        /// Take a key and a pointer from the right sibling
        /// </summary>
        /// <param name="internalNode">internal node</param>
        private static void LendRightSibling(
            BPlusTreeInternalNodeData internalNode,
            int m)
        {
            BPlusTreeInternalNodeData parent = internalNode.parent;
            
            BPlusTreeInternalNodeData sibling = internalNode.rightSibling;
            
            //Find leftmost key and pointer in right sibling
            int borrowedKey = sibling.keys[0];
                
            BPlusTreeNodeData pointer = sibling.childPointers[0];

            //Set leftmost key and pointer from sibling as rightmost key and pointer for this node
            internalNode.keys[internalNode.degree - 1] = parent.keys[0]; //parent's leftmost key? wtf?
                
            internalNode.childPointers[internalNode.degree] = pointer;

            parent.keys[0] = borrowedKey;

            //Remove leftmost key and pointer from right sibling
            BPlusTreeInternalNode.RemovePointer(
                sibling,
                0);
                
            Array.Sort(sibling.keys);
                
            BPlusTreeInternalNode.RemovePointer(
                sibling,
                0);
            
            //Wtf?
            ShiftDown(
                internalNode.childPointers,
                1,
                m);
        }

        private static void ShiftDown(
            BPlusTreeNodeData[] pointers,
            int amount,
            int m)
        {
            BPlusTreeNodeData[] newPointers = new BPlusTreeNodeData[m + 1];
            
            for (int i = amount; i < pointers.Length; i++)
            {
                newPointers[i - amount] = pointers[i];
            }
            
            pointers = newPointers;
        }
        
        private static void MergeWithRightSibling(
            BPlusTreeInternalNodeData internalNode)
        {
            BPlusTreeInternalNodeData parent = internalNode.parent;
            
            BPlusTreeInternalNodeData sibling = internalNode.rightSibling;
            
            //WTF?
            sibling.keys[sibling.degree - 1] = parent.keys[parent.degree - 2];
                
            Array.Sort(sibling.keys, 0, sibling.degree);
            
            //WTF?
            parent.keys[parent.degree - 2] = -1; //null;


            for (int i = 0; i < internalNode.childPointers.Length; i++)
            {
                if (internalNode.childPointers[i] != null)
                {
                    BPlusTreeInternalNode.PrependChildPointer(
                        sibling,
                        internalNode.childPointers[i]);
                    
                    internalNode.childPointers[i].parent = sibling;
                    
                    BPlusTreeInternalNode.RemovePointer(
                        internalNode,
                        i);
                }
            }

            BPlusTreeInternalNode.RemovePointer(
                parent,
                internalNode);

            sibling.leftSibling = internalNode.leftSibling;
        }

        #endregion
    }
}