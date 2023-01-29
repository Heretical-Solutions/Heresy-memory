using System;
using NUnit.Framework;
using System.Runtime.InteropServices;
using HereticalSolutions.Collections.Unmanaged;

namespace HereticalSolutions.Collections.Tests
{
    public class UnmanagedGenericArrayTests
    {
        [TestCase(5, 0, 1, 2, 3, 4, -5)]
        [TestCase(3, 0, 0, 1, -100, 2, 100)]
        public void Write_Values_ValuesStored(
            int arraySize,
            int index1,
            int element1,
            int index2,
            int element2,
            int index3,
            int element3)
        {
            unsafe
            {
                //Allocation
                int elementSize = sizeof(int);
                
                int elementCapacity = arraySize;
                
                int memorySize = elementSize * elementCapacity;
                
                var allocationPointer = (int*)Marshal.AllocHGlobal(memorySize);
                
                UnmanagedGenericArray<int> unmanagedArray = new UnmanagedGenericArray<int>(
                    allocationPointer,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation
                
                //Cache references to unmanaged array elements
                ref int reference1 = ref unmanagedArray[index1];
                
                ref int reference2 = ref unmanagedArray[index2];
                
                ref int reference3 = ref unmanagedArray[index3];
                
                
                //Cache pointers to unmanaged array elements
                int* pointer1 = unmanagedArray.GetPointer(index1);
                
                int* pointer2 = unmanagedArray.GetPointer(index2);
                
                int* pointer3 = unmanagedArray.GetPointer(index3);
                
                
                //Store elements
                unmanagedArray[index1] = element1;
                
                unmanagedArray[index2] = element2;
                
                unmanagedArray[index3] = element3;
                
                //Comparison
                
                //Elements obtained by indexer should be equal to the input
                Assert.AreEqual(unmanagedArray[index1], element1);
                
                Assert.AreEqual(unmanagedArray[index2], element2);
                
                Assert.AreEqual(unmanagedArray[index3], element3);
                
                
                //Elements obtained by pointer method should be equal to the input
                Assert.AreEqual(*unmanagedArray.GetPointer(index1), element1);
                
                Assert.AreEqual(*unmanagedArray.GetPointer(index2), element2);
                
                Assert.AreEqual(*unmanagedArray.GetPointer(index3), element3);
                
                
                //Values behind references should be equal to the input
                Assert.AreEqual(reference1, element1);
                
                Assert.AreEqual(reference2, element2);
                
                Assert.AreEqual(reference3, element3);
                
                
                //Values behind pointers should be equal to the input
                Assert.AreEqual(*pointer1, element1);
                
                Assert.AreEqual(*pointer2, element2);
                
                Assert.AreEqual(*pointer3, element3);
                
                
                //Array indexes obtained from pointers should match the input
                Assert.AreEqual(unmanagedArray.IndexOfPointer(pointer1), index1);
                
                Assert.AreEqual(unmanagedArray.IndexOfPointer(pointer2), index2);
                
                Assert.AreEqual(unmanagedArray.IndexOfPointer(pointer3), index3);
                
                
                //Deallocation
                Marshal.FreeHGlobal((IntPtr)allocationPointer);
            }
        }
    
        [TestCase(3, 1, 1, 3, -5)]
        [TestCase(1, 0, 0, -100, 100)]
        public void Overwrite_Values_ValuesOverwritten(
                int arraySize,
                int index,
                int element1,
                int element2,
                int element3)
        {
            unsafe
            {
                //Allocation
                int elementSize = sizeof(int);
                    
                int elementCapacity = arraySize;
                    
                int memorySize = elementSize * elementCapacity;
                    
                var allocationPointer = (int*)Marshal.AllocHGlobal(memorySize);
                    
                UnmanagedGenericArray<int> unmanagedArray = new UnmanagedGenericArray<int>(
                    allocationPointer,
                    memorySize,
                    elementSize,
                    elementCapacity);
                    
                //Operation and comparison
                
                //Cache reference and pointer to unmanaged array element
                ref int reference = ref unmanagedArray[index];
                
                int* pointer = unmanagedArray.GetPointer(index);
                
                
                //Overwrite element at index with the input and perform assertions
                OverwriteAndAssert(unmanagedArray, index, element1, ref reference, pointer);
                
                OverwriteAndAssert(unmanagedArray, index, element2, ref reference, pointer);
                
                OverwriteAndAssert(unmanagedArray, index, element3, ref reference, pointer);
                
                
                //Deallocation
                Marshal.FreeHGlobal((IntPtr)allocationPointer);
            }
        }
        
        /// <summary>
        /// Overwrite value in the unmanaged array at given index and perform assertions
        /// </summary>
        /// <param name="unmanagedArray">Unmanaged array</param>
        /// <param name="index">Target index</param>
        /// <param name="element">Element value</param>
        /// <param name="reference">Element reference</param>
        /// <param name="pointer">Element pointer</param>
        private unsafe void OverwriteAndAssert(
            UnmanagedGenericArray<int> unmanagedArray,
            int index,
            int element,
            ref int reference,
            int* pointer)
        {
            //Overwrite element
            unmanagedArray[index] = element;
            
            //Element obtained by indexer should be equal to the input    
            Assert.AreEqual(unmanagedArray[index], element);
            
            //Element obtained by pointer method should be equal to the input
            Assert.AreEqual(*unmanagedArray.GetPointer(index), element);
            
            //Value behind reference should be equal to the input
            Assert.AreEqual(reference, element);
            
            //Value behind pointer should be equal to the input
            Assert.AreEqual(*pointer, element);
            
            //Array index obtained from pointer should match the input
            Assert.AreEqual(unmanagedArray.IndexOfPointer(pointer), index);
        }
    }
}