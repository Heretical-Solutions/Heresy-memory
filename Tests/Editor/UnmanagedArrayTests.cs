using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Memory.Tests
{
    public class UnmanagedArrayTests
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
                
                var pointer = Marshal.AllocHGlobal(memorySize);
                
                UnmanagedArray unmanagedArray = new UnmanagedArray(
                    (byte*) pointer,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation
                
                //Cache pointers to unmanaged array elements
                void* pointer1 = unmanagedArray[index1];
                
                void* pointer2 = unmanagedArray[index2];
                
                void* pointer3 = unmanagedArray[index3];
                
                
                //Cache generic pointers to unmanaged array elements
                int* genericPointer1 = unmanagedArray.Get<int>(index1);
                
                int* genericPointer2 = unmanagedArray.Get<int>(index2);
                
                int* genericPointer3 = unmanagedArray.Get<int>(index3);
                
                
                //Store elements
                *(int*)(unmanagedArray[index1]) = element1;
                
                *(int*)(unmanagedArray[index2]) = element2;
                
                *(int*)(unmanagedArray[index3]) = element3;
                
                //Comparison
                
                //Elements obtained by indexer should be equal to the input
                Assert.AreEqual(*(int*)(unmanagedArray[index1]), element1);
                
                Assert.AreEqual(*(int*)(unmanagedArray[index2]), element2);
                
                Assert.AreEqual(*(int*)(unmanagedArray[index3]), element3);
                
                
                //Elements obtained by generic method should be equal to the input
                Assert.AreEqual(*unmanagedArray.Get<int>(index1), element1);
                
                Assert.AreEqual(*unmanagedArray.Get<int>(index2), element2);
                
                Assert.AreEqual(*unmanagedArray.Get<int>(index3), element3);
                
                
                //Values behind pointers should be equal to the input
                Assert.AreEqual(*(int*)pointer1, element1);
                
                Assert.AreEqual(*(int*)pointer2, element2);
                
                Assert.AreEqual(*(int*)pointer3, element3);
                
                
                //Values behind generic pointers should be equal to the input
                Assert.AreEqual(*genericPointer1, element1);
                
                Assert.AreEqual(*genericPointer2, element2);
                
                Assert.AreEqual(*genericPointer3, element3);
                
                
                //Array indexes obtained from pointers should match the input
                Assert.AreEqual(unmanagedArray.IndexOf(pointer1), index1);
                
                Assert.AreEqual(unmanagedArray.IndexOf(pointer2), index2);
                
                Assert.AreEqual(unmanagedArray.IndexOf(pointer3), index3);
                
                
                //Array indexes obtained from generic pointers should match the input
                Assert.AreEqual(unmanagedArray.IndexOf<int>(genericPointer1), index1);
                
                Assert.AreEqual(unmanagedArray.IndexOf<int>(genericPointer2), index2);
                
                Assert.AreEqual(unmanagedArray.IndexOf<int>(genericPointer3), index3);
                
                
                //Deallocation
                Marshal.FreeHGlobal(pointer);
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
                
                var pointer = Marshal.AllocHGlobal(memorySize);
                
                UnmanagedArray unmanagedArray = new UnmanagedArray(
                    (byte*) pointer,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation and comparison
                
                //Cache pointer and generic pointer to unmanaged array element
                void* pointerToIndex = unmanagedArray[index];
                
                int* genericPointer = unmanagedArray.Get<int>(index);
                
                
                //Overwrite element at index with the input and perform assertions
                OverwriteAndAssert(unmanagedArray, index, element1, pointerToIndex, genericPointer);
                
                OverwriteAndAssert(unmanagedArray, index, element2, pointerToIndex, genericPointer);
                
                OverwriteAndAssert(unmanagedArray, index, element3, pointerToIndex, genericPointer);
                
                //Deallocation
                Marshal.FreeHGlobal(pointer);
            }
        }
        
        /// <summary>
        /// Overwrite value in the unmanaged array at given index and perform assertions
        /// </summary>
        /// <param name="unmanagedArray">Unmanaged array</param>
        /// <param name="index">Target index</param>
        /// <param name="element">Element value</param>
        /// <param name="pointerToIndex">Element pointer</param>
        /// <param name="genericPointer">Generic element pointer</param>
        private unsafe void OverwriteAndAssert(
            UnmanagedArray unmanagedArray,
            int index,
            int element,
            void* pointerToIndex,
            int* genericPointer)
        {
            //Overwrite element
            *(int*)(unmanagedArray[index]) = element;
                
            //Element obtained by indexer should be equal to the input
            Assert.AreEqual(*(int*)(unmanagedArray[index]), element);
            
            //Element obtained by generic method should be equal to the input
            Assert.AreEqual(*unmanagedArray.Get<int>(index), element);
            
            //Value behind pointer should be equal to the input
            Assert.AreEqual(*(int*)pointerToIndex, element);
            
            //Value behind generic pointer should be equal to the input
            Assert.AreEqual(*genericPointer, element);
            
            //Array index obtained from pointer should match the input
            Assert.AreEqual(unmanagedArray.IndexOf(pointerToIndex), index);
            
            //Array index obtained from generic pointer should match the input
            Assert.AreEqual(unmanagedArray.IndexOf<int>(genericPointer), index);
        }
        
        //Do not try this shit. It gives a green arrow on test run and even if test fails after await the green arrow remains
        /*
        [TestCase(5, 0, 1, 2, 3, 4, -5)]
        [TestCase(3, 0, 0, 1, -100, 2, 100)]
        public async void Add_Values_Wait_1_Second_ValuesStillPresent(
            int arraySize,
            int index1,
            int element1,
            int index2,
            int element2,
            int index3,
            int element3)
        {
            UnmanagedArray unmanagedArray;
            
            IntPtr pointer;
            
            unsafe
            {
                //Allocation
                int elementSize = sizeof(int);
                
                int elementCapacity = arraySize;
                
                int memorySize = elementSize * elementCapacity;
                
                pointer = Marshal.AllocHGlobal(memorySize);
                
                unmanagedArray = new UnmanagedArray(
                    (byte*) pointer,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation
                *(int*)(unmanagedArray[index1]) = element1;
                
                *(int*)(unmanagedArray[index2]) = element2;
                
                *(int*)(unmanagedArray[index3]) = element3;
            }
            
            await Task.Delay(1000);
            
            unsafe
            {
                //Comparison
                Assert.AreEqual(*(int*)(unmanagedArray[index1]), element1);
                
                Assert.AreEqual(*(int*)(unmanagedArray[index2]), element2);
                
                Assert.AreEqual(*(int*)(unmanagedArray[index3]), element3);
                
                //Deallocation
                Marshal.FreeHGlobal(pointer);
            }
        }
        */
    }
}