using System;
using NUnit.Framework;
using System.Runtime.InteropServices;
using HereticalSolutions.Collections.Unmanaged;

namespace HereticalSolutions.Collections.Tests
{
    public class GenericPackedArrayTests
    {
        [TestCase(5, 1, 3, -5)]
        [TestCase(3, 0, -100, 100)]
        public void Push_Values_ValuesStored(
            int arraySize,
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
                
                var allocationPointer1 = (int*)Marshal.AllocHGlobal(memorySize);
                
                var allocationPointer2 = (int*)Marshal.AllocHGlobal(memorySize);

                GenericPackedArray<int> packedArray1 = new GenericPackedArray<int>(
                    allocationPointer1,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                GenericPackedArray<int> packedArray2 = new GenericPackedArray<int>(
                    allocationPointer2,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation
                
                //Store elements in packedArray1
                //Cache references to unmanaged array elements
                
                ref int reference1 = ref packedArray1.Pop(out int referenceIndex1);
                
                reference1 = element1;
                
                
                ref int reference2 = ref packedArray1.Pop(out int referenceIndex2);
                
                reference2 = element2;
                
                
                ref int reference3 = ref packedArray1.Pop(out int referenceIndex3);
                
                reference3 = element3;
                
                
                //Store elements in packedArray2
                //Cache pointers to unmanaged array elements
                
                int* pointer1 = packedArray2.PopPointer();
                
                *pointer1 = element1;
                
                
                int* pointer2 = packedArray2.PopPointer();
                
                *pointer2 = element2;
                
                
                int* pointer3 = packedArray2.PopPointer();
                
                *pointer3 = element3;
                
                
                //Comparison
                
                //Count of elements stored should be equal to the input
                Assert.AreEqual(packedArray1.Count, 3);
                
                Assert.AreEqual(packedArray2.Count, 3);
                
                
                //If allocated space is more than element count then there should be some free space left
                if (arraySize == 3)
                {
                    Assert.IsFalse(packedArray1.HasFreeSpace);
                    
                    Assert.IsFalse(packedArray2.HasFreeSpace);
                }
                else
                {
                    Assert.IsTrue(packedArray1.HasFreeSpace);
                    
                    Assert.IsTrue(packedArray2.HasFreeSpace);
                }
                
                
                //Elements obtained by indexer should be equal to the input
                Assert.AreEqual(packedArray1[0], element1);
                
                Assert.AreEqual(packedArray1[1], element2);
                
                Assert.AreEqual(packedArray1[2], element3);
                
                
                Assert.AreEqual(packedArray2[0], element1);
                
                Assert.AreEqual(packedArray2[1], element2);
                
                Assert.AreEqual(packedArray2[2], element3);
                
                
                //Elements obtained by pointer method should be equal to the input
                Assert.AreEqual(*packedArray1.GetPointer(0), element1);
                
                Assert.AreEqual(*packedArray1.GetPointer(1), element2);
                
                Assert.AreEqual(*packedArray1.GetPointer(2), element3);
                
                
                Assert.AreEqual(*packedArray2.GetPointer(0), element1);
                
                Assert.AreEqual(*packedArray2.GetPointer(1), element2);
                
                Assert.AreEqual(*packedArray2.GetPointer(2), element3);
                
                
                //Values behind references should be equal to the input
                Assert.AreEqual(reference1, element1);
                
                Assert.AreEqual(reference2, element2);
                
                Assert.AreEqual(reference3, element3);
                
                
                //Values behind pointers should be equal to the input
                Assert.AreEqual(*pointer1, element1);
                
                Assert.AreEqual(*pointer2, element2);
                
                Assert.AreEqual(*pointer3, element3);


				//Array indexes obtained from references should match the input
				Assert.AreEqual(referenceIndex1, 0);

				Assert.AreEqual(referenceIndex2, 1);

				Assert.AreEqual(referenceIndex3, 2);

                
                //Array indexes obtained from pointers should match the input
                Assert.AreEqual(packedArray2.IndexOfPointer(pointer1), 0);
                
                Assert.AreEqual(packedArray2.IndexOfPointer(pointer2), 1);
                
                Assert.AreEqual(packedArray2.IndexOfPointer(pointer3), 2);
                
                
                //Deallocation
                Marshal.FreeHGlobal((IntPtr)allocationPointer1);
                
                Marshal.FreeHGlobal((IntPtr)allocationPointer2);
            }
        }
        
        [TestCase(3, 1, 3, -5)]
        [TestCase(1, 0, -100, 100)]
        public void Push_Values_ValuesStored_Pop_Values_ValuesErased(
            int arraySize,
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
                
                var allocationPointer1 = (int*)Marshal.AllocHGlobal(memorySize);
                
                var allocationPointer2 = (int*)Marshal.AllocHGlobal(memorySize);
                
                GenericPackedArray<int> packedArray1 = new GenericPackedArray<int>(
                    allocationPointer1,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                GenericPackedArray<int> packedArray2 = new GenericPackedArray<int>(
                    allocationPointer2,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation and comparison
                
                //Push/pop elements from the input and perform assertions
                PushAssertPopAssert(packedArray1, packedArray2, arraySize, element1);
                
                PushAssertPopAssert(packedArray1, packedArray2, arraySize, element2);
                
                PushAssertPopAssert(packedArray1, packedArray2, arraySize, element3);
                
                //Deallocation
                Marshal.FreeHGlobal((IntPtr)allocationPointer1);
                
                Marshal.FreeHGlobal((IntPtr)allocationPointer2);
            }
        }
        
        /// <summary>
        /// Push value into empty packed array, perform assertion, pop value back and perform assertion again
        /// </summary>
        /// <param name="packedArray1">Packed array 1</param>
        /// <param name="packedArray2">Packed array 2</param>
        /// <param name="arraySize">Array size</param>
        /// <param name="element">Element value</param>
        private unsafe void PushAssertPopAssert(
            GenericPackedArray<int> packedArray1,
            GenericPackedArray<int> packedArray2,
            int arraySize,
            int element)
        {
            //Store element1 in packedArray1
            //Cache pointer to unmanaged array element
                
            ref int reference = ref packedArray1.Pop(out int referenceIndex);

			reference = element;
                
                
            //Store element1 in packedArray2
            //Cache pointer to unmanaged array element
                
            int* pointer = packedArray2.PopPointer();
                
            *pointer = element;
                
                
            //Count of elements stored should be equal to 1
            Assert.AreEqual(packedArray1.Count, 1);
                
            Assert.AreEqual(packedArray2.Count, 1);
                
                
            //If allocated space is more than element count then there should be some free space left
            if (arraySize == 1)
            {
                Assert.IsFalse(packedArray1.HasFreeSpace);
                
                Assert.IsFalse(packedArray2.HasFreeSpace);
            }
            else
            {
                Assert.IsTrue(packedArray1.HasFreeSpace);
                
                Assert.IsTrue(packedArray2.HasFreeSpace);
            }
                
            //Element obtained by indexer should be equal to the input
            Assert.AreEqual(packedArray1[0], element);
                
            Assert.AreEqual(packedArray2[0], element);
                
                
            //Element obtained by pointer method should be equal to the input
            Assert.AreEqual(*packedArray1.GetPointer(0), element);
                
            Assert.AreEqual(*packedArray2.GetPointer(0), element);
                
                
            //Value behind reference should be equal to the input
            Assert.AreEqual(reference, element);
                
                
            //Value behind pointer should be equal to the input
            Assert.AreEqual(*pointer, element);


			//Array index obtained from reference should be 0
			Assert.AreEqual(referenceIndex, 0);

            
            //Array index obtained from pointer should be 0
            Assert.AreEqual(packedArray2.IndexOfPointer(pointer), 0);
                
                
            //Remove elements from packedArray1 and packedArray2
            packedArray1.PushIndex(referenceIndex);
                
            packedArray2.PushPointer(pointer);
                
                
            //Count of elements stored should be equal to 0
            Assert.AreEqual(packedArray1.Count, 0);
                
            Assert.AreEqual(packedArray2.Count, 0);
                
                
            //There definitely should be some free space left
            Assert.IsTrue(packedArray1.HasFreeSpace);
                    
            Assert.IsTrue(packedArray2.HasFreeSpace);
        }
        
        [TestCase(5, 1, 3, -5)]
        [TestCase(3, 0, -100, 100)]
        public void Push_Values_Pop_Then_Push_Values_ValuesReordered(
            int arraySize,
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
                
                var allocationPointer1 = (int*)Marshal.AllocHGlobal(memorySize);
                
                var allocationPointer2 = (int*)Marshal.AllocHGlobal(memorySize);
                
                GenericPackedArray<int> packedArray1 = new GenericPackedArray<int>(
                    allocationPointer1,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                GenericPackedArray<int> packedArray2 = new GenericPackedArray<int>(
                    allocationPointer2,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation and comparison
                
                //Store elements in packedArray1
                //Cache pointers to unmanaged array elements
                
                ref int reference1 = ref packedArray1.Pop(out int referenceIndex1);
                
                reference1 = element1;
                
                
                ref int reference2 = ref packedArray1.Pop(out int referenceIndex2);
                
                reference2 = element2;
                
                
                ref int reference3 = ref packedArray1.Pop(out int referenceIndex3);
                
                reference3 = element3;
                
                
                //Store elements in packedArray2
                //Cache pointers to unmanaged array elements
                
                int* pointer1 = packedArray2.PopPointer();
                
                *pointer1 = element1;
                
                
                int* pointer2 = packedArray2.PopPointer();
                
                *pointer2 = element2;
                
                
                int* pointer3 = packedArray2.PopPointer();
                
                *pointer3 = element3;
                
                
                //Pop/push elements in the arrays and perform assertions
                PopAssertPushAssert(
                    packedArray1,
                    packedArray2,
                    arraySize,
                    
                    1,
                    
                    ref reference1,
                    ref reference2,
                    ref reference3,

                    referenceIndex1,
					referenceIndex2,
					referenceIndex3,
                    
                    pointer1,
                    pointer2,
                    pointer3,
                    
                    element1,
                    element3,
                    element2);
                
                PopAssertPushAssert(
                    packedArray1,
                    packedArray2,
                    arraySize,
                    
                    0,

					ref reference1,
					ref reference2,
					ref reference3,

					referenceIndex1,
					referenceIndex2,
					referenceIndex3,
                    
                    pointer1,
                    pointer2,
                    pointer3,
                    
                    element2,
                    element3,
                    element1);
                
                PopAssertPushAssert(
                    packedArray1,
                    packedArray2,
                    arraySize,
                    
                    2,

					ref reference1,
					ref reference2,
					ref reference3,

					referenceIndex1,
					referenceIndex2,
					referenceIndex3,
                    
                    pointer1,
                    pointer2,
                    pointer3,
                    
                    element2,
                    element3,
                    element1);
                
                
                //Deallocation
                Marshal.FreeHGlobal((IntPtr)allocationPointer1);
                
                Marshal.FreeHGlobal((IntPtr)allocationPointer2);
            }
        }
        
        /// <summary>
        /// Pop element from packed arrays at given index, perform assertions, then push it back and perform assertions again
        /// </summary>
        /// <param name="packedArray1">Packed array 1</param>
        /// <param name="packedArray2">Packed array 2</param>
        /// <param name="arraySize">Array size</param>
        /// <param name="index">Target element index</param>
        /// <param name="expectedElement1">Expected value of element 1 after the push</param>
        /// <param name="expectedElement2">Expected value of element 2 after the push</param>
        /// <param name="expectedElement3">Expected value of element 3 after the push</param>
        private unsafe void PopAssertPushAssert(
            GenericPackedArray<int> packedArray1,
            GenericPackedArray<int> packedArray2,
            
            int arraySize,
            int index,
            
            ref int reference1,
            ref int reference2,
            ref int reference3,

            int referenceIndex1,
			int referenceIndex2,
			int referenceIndex3,
            
            int* pointer1,
            int* pointer2,
            int* pointer3,
            
            int expectedElement1,
            int expectedElement2,
            int expectedElement3)
        {
            //Pop elements by target index in packed arrays
            //Return values should be equal to 2 (assuming there were only 3 elements stored)
            int returnValue1 = packedArray1.PushIndex(
                (index == 0)
                    ? referenceIndex1 
                    : (index == 1)
                        ? referenceIndex2
                        : referenceIndex3);
            
            Assert.AreEqual(returnValue1, (index == 2) ? -1 : 2);
            
            int returnValue2 = packedArray2.PushPointer(
                (index == 0)
                    ? pointer1
                    : (index == 1)
                        ? pointer2
                        : pointer3);
            
            Assert.AreEqual(returnValue2, (index == 2) ? -1 : 2);
            
            
            //Count of elements stored should be equal to 2 (assuming there were only 3 elements stored)
            Assert.AreEqual(packedArray1.Count, 2);
                
            Assert.AreEqual(packedArray2.Count, 2);
                
                
            //There definitely should be some free space left
            Assert.IsTrue(packedArray1.HasFreeSpace);
                    
            Assert.IsTrue(packedArray2.HasFreeSpace);
            
                
            //Elements obtained by indexer should be equal to the input
            Assert.AreEqual(packedArray1[0], expectedElement1);
            
            Assert.AreEqual(packedArray1[1], expectedElement2);
            
            
            Assert.AreEqual(packedArray2[0], expectedElement1);
            
            Assert.AreEqual(packedArray2[1], expectedElement2);
            
            
            //Elements obtained by pointer method should be equal to the input
            Assert.AreEqual(*packedArray1.GetPointer(0), expectedElement1);
            
            Assert.AreEqual(*packedArray1.GetPointer(1), expectedElement2);
            
            
            Assert.AreEqual(*packedArray2.GetPointer(0), expectedElement1);
            
            Assert.AreEqual(*packedArray2.GetPointer(1), expectedElement2);
                
                
            //Values behind references should be equal to the input
            Assert.AreEqual(reference1, expectedElement1);
            
            Assert.AreEqual(reference2, expectedElement2);
                
                
            //Value behind pointers should be equal to the input
            Assert.AreEqual(*pointer1, expectedElement1);
            
            Assert.AreEqual(*pointer2, expectedElement2);


			//Array indexes obtained from pointers should be equal to the input
			Assert.AreEqual(referenceIndex1, 0);

			Assert.AreEqual(referenceIndex2, 1);

			Assert.AreEqual(referenceIndex3, 2);
            

            //Array indexes obtained from pointers should be equal to the input
            Assert.AreEqual(packedArray2.IndexOfPointer(pointer1), 0);
            
            Assert.AreEqual(packedArray2.IndexOfPointer(pointer2), 1);
            
            Assert.AreEqual(packedArray2.IndexOfPointer(pointer3), 2);
            
            
            //Push popped elements back into packed arrays
            
            //Cache pointer to unmanaged array element
            ref int reference = ref packedArray1.Pop(out int referenceIndex);

			reference = expectedElement3;
            
            //Cache pointer to unmanaged array element
                
            int* pointer = packedArray2.PopPointer();
                
            *pointer = expectedElement3;
            
                
            //Count of elements stored should be equal to 3 (assuming there were only 3 elements stored)
            Assert.AreEqual(packedArray1.Count, 3);
                
            Assert.AreEqual(packedArray2.Count, 3);
                
                
            //If allocated space is more than element count then there should be some free space left
            if (arraySize == 3)
            {
                Assert.IsFalse(packedArray1.HasFreeSpace);
                
                Assert.IsFalse(packedArray2.HasFreeSpace);
            }
            else
            {
                Assert.IsTrue(packedArray1.HasFreeSpace);
                
                Assert.IsTrue(packedArray2.HasFreeSpace);
            }
            
                
            //Elements obtained by indexer should be equal to the input
            Assert.AreEqual(packedArray1[0], expectedElement1);
            
            Assert.AreEqual(packedArray1[1], expectedElement2);
            
            Assert.AreEqual(packedArray1[2], expectedElement3);
            
            
            Assert.AreEqual(packedArray2[0], expectedElement1);
            
            Assert.AreEqual(packedArray2[1], expectedElement2);
            
            Assert.AreEqual(packedArray2[2], expectedElement3);
            
            
            //Elements obtained by pointer method should be equal to the input
            Assert.AreEqual(*packedArray1.GetPointer(0), expectedElement1);
            
            Assert.AreEqual(*packedArray1.GetPointer(1), expectedElement2);
            
            Assert.AreEqual(*packedArray1.GetPointer(2), expectedElement3);
            
            
            Assert.AreEqual(*packedArray2.GetPointer(0), expectedElement1);
            
            Assert.AreEqual(*packedArray2.GetPointer(1), expectedElement2);
            
            Assert.AreEqual(*packedArray2.GetPointer(2), expectedElement3);
                
                
            //Values behind references should be equal to the input
            Assert.AreEqual(reference1, expectedElement1);
            
            Assert.AreEqual(reference2, expectedElement2);
            
            Assert.AreEqual(reference3, expectedElement3);
            
            Assert.AreEqual(reference, expectedElement3);
                
                
            //Value behind pointers should be equal to the input
            Assert.AreEqual(*pointer1, expectedElement1);
            
            Assert.AreEqual(*pointer2, expectedElement2);
            
            Assert.AreEqual(*pointer3, expectedElement3);
            
            Assert.AreEqual(*pointer, expectedElement3);
                
                
            //Array indexes obtained from references should be equal to the input
            Assert.AreEqual(referenceIndex1, 0);
            
            Assert.AreEqual(referenceIndex2, 1);
            
            Assert.AreEqual(referenceIndex3, 2);

			Assert.AreEqual(referenceIndex, 2);
                
                
            //Array indexes obtained from pointers should be equal to the input
            Assert.AreEqual(packedArray2.IndexOfPointer(pointer1), 0);
            
            Assert.AreEqual(packedArray2.IndexOfPointer(pointer2), 1);
            
            Assert.AreEqual(packedArray2.IndexOfPointer(pointer3), 2);

			Assert.AreEqual(packedArray2.IndexOfPointer(pointer), 2);
        }
    }
}