using NUnit.Framework;
using System.Runtime.InteropServices;
using HereticalSolutions.Collections.Unmanaged;

namespace HereticalSolutions.Collections.Tests
{
    public class PackedArrayTests
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
                
                var allocationPointer1 = Marshal.AllocHGlobal(memorySize);
                
                var allocationPointer2 = Marshal.AllocHGlobal(memorySize);
                
                PackedArray packedArray1 = new PackedArray(
                    (byte*) allocationPointer1,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                PackedArray packedArray2 = new PackedArray(
                    (byte*) allocationPointer2,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation
                
                //Store elements in packedArray1
                //Cache pointers to unmanaged array elements
                
                void* pointer1 = packedArray1.PopPointer();
                
                *(int*)pointer1 = element1;
                
                
                void* pointer2 = packedArray1.PopPointer();
                
                *(int*)pointer2 = element2;
                
                
                void* pointer3 = packedArray1.PopPointer();
                
                *(int*)pointer3 = element3;
                
                
                //Store elements in packedArray2
                //Cache generic pointers to unmanaged array elements
                
                int* genericPointer1 = packedArray2.PopGeneric<int>();
                
                *genericPointer1 = element1;
                
                
                int* genericPointer2 = packedArray2.PopGeneric<int>();
                
                *genericPointer2 = element2;
                
                
                int* genericPointer3 = packedArray2.PopGeneric<int>();
                
                *genericPointer3 = element3;
                
                
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
                Assert.AreEqual(*(int*)(packedArray1[0]), element1);
                
                Assert.AreEqual(*(int*)(packedArray1[1]), element2);
                
                Assert.AreEqual(*(int*)(packedArray1[2]), element3);
                
                
                Assert.AreEqual(*(int*)(packedArray2[0]), element1);
                
                Assert.AreEqual(*(int*)(packedArray2[1]), element2);
                
                Assert.AreEqual(*(int*)(packedArray2[2]), element3);
                
                
                //Elements obtained by generic method should be equal to the input
                Assert.AreEqual(*packedArray1.GetGeneric<int>(0), element1);
                
                Assert.AreEqual(*packedArray1.GetGeneric<int>(1), element2);
                
                Assert.AreEqual(*packedArray1.GetGeneric<int>(2), element3);
                
                
                Assert.AreEqual(*packedArray2.GetGeneric<int>(0), element1);
                
                Assert.AreEqual(*packedArray2.GetGeneric<int>(1), element2);
                
                Assert.AreEqual(*packedArray2.GetGeneric<int>(2), element3);
                
                
                //Values behind pointers should be equal to the input
                Assert.AreEqual(*(int*)pointer1, element1);
                
                Assert.AreEqual(*(int*)pointer2, element2);
                
                Assert.AreEqual(*(int*)pointer3, element3);
                
                
                //Values behind generic pointers should be equal to the input
                Assert.AreEqual(*genericPointer1, element1);
                
                Assert.AreEqual(*genericPointer2, element2);
                
                Assert.AreEqual(*genericPointer3, element3);
                
                
                //Array indexes obtained from pointers should match the input
                Assert.AreEqual(packedArray1.IndexOfPointer(pointer1), 0);
                
                Assert.AreEqual(packedArray1.IndexOfPointer(pointer2), 1);
                
                Assert.AreEqual(packedArray1.IndexOfPointer(pointer3), 2);
                
                
                //Array indexes obtained from generic pointers should match the input
                Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer1), 0);
                
                Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer2), 1);
                
                Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer3), 2);
                
                
                //Deallocation
                Marshal.FreeHGlobal(allocationPointer1);
                
                Marshal.FreeHGlobal(allocationPointer2);
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
                
                var allocationPointer1 = Marshal.AllocHGlobal(memorySize);
                
                var allocationPointer2 = Marshal.AllocHGlobal(memorySize);
                
                PackedArray packedArray1 = new PackedArray(
                    (byte*) allocationPointer1,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                PackedArray packedArray2 = new PackedArray(
                    (byte*) allocationPointer2,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation and comparison
                
                //Push/pop elements from the input and perform assertions
                PushAssertPopAssert(packedArray1, packedArray2, arraySize, element1);
                
                PushAssertPopAssert(packedArray1, packedArray2, arraySize, element2);
                
                PushAssertPopAssert(packedArray1, packedArray2, arraySize, element3);
                
                //Deallocation
                Marshal.FreeHGlobal(allocationPointer1);
                
                Marshal.FreeHGlobal(allocationPointer2);
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
            PackedArray packedArray1,
            PackedArray packedArray2,
            int arraySize,
            int element)
        {
            //Store element1 in packedArray1
            //Cache pointer to unmanaged array element
                
            void* pointer = packedArray1.PopPointer();
                
            *(int*)pointer = element;
                
                
            //Store element1 in packedArray2
            //Cache generic pointer to unmanaged array element
                
            int* genericPointer = packedArray2.PopGeneric<int>();
                
            *genericPointer = element;
                
                
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
            Assert.AreEqual(*(int*)(packedArray1[0]), element);
                
            Assert.AreEqual(*(int*)(packedArray2[0]), element);
                
                
            //Element obtained by generic method should be equal to the input
            Assert.AreEqual(*packedArray1.GetGeneric<int>(0), element);
                
            Assert.AreEqual(*packedArray2.GetGeneric<int>(0), element);
                
                
            //Value behind pointer should be equal to the input
            Assert.AreEqual(*(int*)pointer, element);
                
                
            //Value behind generic pointer should be equal to the input
            Assert.AreEqual(*genericPointer, element);
                
                
            //Array index obtained from pointer should be 0
            Assert.AreEqual(packedArray1.IndexOfPointer(pointer), 0);
                
                
            //Array index obtained from generic pointer should be 0
            Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer), 0);
                
                
            //Remove elements from packedArray1 and packedArray2
            packedArray1.PushPointer(pointer);
                
            packedArray2.PushGeneric<int>(genericPointer);
                
                
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
                
                var allocationPointer1 = Marshal.AllocHGlobal(memorySize);
                
                var allocationPointer2 = Marshal.AllocHGlobal(memorySize);
                
                PackedArray packedArray1 = new PackedArray(
                    (byte*) allocationPointer1,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                PackedArray packedArray2 = new PackedArray(
                    (byte*) allocationPointer2,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation and comparison
                
                //Store elements in packedArray1
                //Cache pointers to unmanaged array elements
                
                void* pointer1 = packedArray1.PopPointer();
                
                *(int*)pointer1 = element1;
                
                
                void* pointer2 = packedArray1.PopPointer();
                
                *(int*)pointer2 = element2;
                
                
                void* pointer3 = packedArray1.PopPointer();
                
                *(int*)pointer3 = element3;
                
                
                //Store elements in packedArray2
                //Cache generic pointers to unmanaged array elements
                
                int* genericPointer1 = packedArray2.PopGeneric<int>();
                
                *genericPointer1 = element1;
                
                
                int* genericPointer2 = packedArray2.PopGeneric<int>();
                
                *genericPointer2 = element2;
                
                
                int* genericPointer3 = packedArray2.PopGeneric<int>();
                
                *genericPointer3 = element3;
                
                
                //Pop/push elements in the arrays and perform assertions
                PopAssertPushAssert(
                    packedArray1,
                    packedArray2,
                    arraySize,
                    
                    1,
                    
                    pointer1,
                    pointer2,
                    pointer3,
                    
                    genericPointer1,
                    genericPointer2,
                    genericPointer3,
                    
                    element1,
                    element3,
                    element2);
                
                PopAssertPushAssert(
                    packedArray1,
                    packedArray2,
                    arraySize,
                    
                    0,
                    
                    pointer1,
                    pointer2,
                    pointer3,
                    
                    genericPointer1,
                    genericPointer2,
                    genericPointer3,
                    
                    element2,
                    element3,
                    element1);
                
                PopAssertPushAssert(
                    packedArray1,
                    packedArray2,
                    arraySize,
                    
                    2,
                    
                    pointer1,
                    pointer2,
                    pointer3,
                    
                    genericPointer1,
                    genericPointer2,
                    genericPointer3,
                    
                    element2,
                    element3,
                    element1);
                
                
                //Deallocation
                Marshal.FreeHGlobal(allocationPointer1);
                
                Marshal.FreeHGlobal(allocationPointer2);
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
            PackedArray packedArray1,
            PackedArray packedArray2,
            
            int arraySize,
            int index,
            
            void* pointer1,
            void* pointer2,
            void* pointer3,
            
            int* genericPointer1,
            int* genericPointer2,
            int* genericPointer3,
            
            int expectedElement1,
            int expectedElement2,
            int expectedElement3)
        {
            //Pop elements by target index in packed arrays
            //Return values should be equal to 2 (assuming there were only 3 elements stored)
            int returnValue1 = packedArray1.PushPointer(
                (index == 0)
                    ? pointer1 
                    : (index == 1)
                        ? pointer2
                        : pointer3);
            
            Assert.AreEqual(returnValue1, (index == 2) ? -1 : 2);
            
            int returnValue2 = packedArray2.PushGeneric<int>(
                (index == 0)
                    ? genericPointer1
                    : (index == 1)
                        ? genericPointer2
                        : genericPointer3);
            
            Assert.AreEqual(returnValue2, (index == 2) ? -1 : 2);
            
            
            //Count of elements stored should be equal to 2 (assuming there were only 3 elements stored)
            Assert.AreEqual(packedArray1.Count, 2);
                
            Assert.AreEqual(packedArray2.Count, 2);
                
                
            //There definitely should be some free space left
            Assert.IsTrue(packedArray1.HasFreeSpace);
                    
            Assert.IsTrue(packedArray2.HasFreeSpace);
            
                
            //Elements obtained by indexer should be equal to the input
            Assert.AreEqual(*(int*)(packedArray1[0]), expectedElement1);
            
            Assert.AreEqual(*(int*)(packedArray1[1]), expectedElement2);
            
            
            Assert.AreEqual(*(int*)(packedArray2[0]), expectedElement1);
            
            Assert.AreEqual(*(int*)(packedArray2[1]), expectedElement2);
            
            
            //Elements obtained by generic method should be equal to the input
            Assert.AreEqual(*packedArray1.GetGeneric<int>(0), expectedElement1);
            
            Assert.AreEqual(*packedArray1.GetGeneric<int>(1), expectedElement2);
            
            
            Assert.AreEqual(*packedArray2.GetGeneric<int>(0), expectedElement1);
            
            Assert.AreEqual(*packedArray2.GetGeneric<int>(1), expectedElement2);
                
                
            //Values behind pointers should be equal to the input
            Assert.AreEqual(*(int*)pointer1, expectedElement1);
            
            Assert.AreEqual(*(int*)pointer2, expectedElement2);
                
                
            //Value behind generic pointer should be equal to the input
            Assert.AreEqual(*genericPointer1, expectedElement1);
            
            Assert.AreEqual(*genericPointer2, expectedElement2);
                
                
            //Array indexes obtained from pointers should be equal to the input
            Assert.AreEqual(packedArray1.IndexOfPointer(pointer1), 0);
            
            Assert.AreEqual(packedArray1.IndexOfPointer(pointer2), 1);
            
            Assert.AreEqual(packedArray1.IndexOfPointer(pointer3), 2);
                
                
            //Array indexes obtained from generic pointers should be equal to the input
            Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer1), 0);
            
            Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer2), 1);
            
            Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer3), 2);
            
            
            //Push popped elements back into packed arrays
            
            //Cache pointer to unmanaged array element
            void* pointer = packedArray1.PopPointer();
                
            *(int*)pointer = expectedElement3;
            
            //Cache generic pointer to unmanaged array element
                
            int* genericPointer = packedArray2.PopGeneric<int>();
                
            *genericPointer = expectedElement3;
            
                
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
            Assert.AreEqual(*(int*)(packedArray1[0]), expectedElement1);
            
            Assert.AreEqual(*(int*)(packedArray1[1]), expectedElement2);
            
            Assert.AreEqual(*(int*)(packedArray1[2]), expectedElement3);
            
            
            Assert.AreEqual(*(int*)(packedArray2[0]), expectedElement1);
            
            Assert.AreEqual(*(int*)(packedArray2[1]), expectedElement2);
            
            Assert.AreEqual(*(int*)(packedArray2[2]), expectedElement3);
            
            
            //Elements obtained by generic method should be equal to the input
            Assert.AreEqual(*packedArray1.GetGeneric<int>(0), expectedElement1);
            
            Assert.AreEqual(*packedArray1.GetGeneric<int>(1), expectedElement2);
            
            Assert.AreEqual(*packedArray1.GetGeneric<int>(2), expectedElement3);
            
            
            Assert.AreEqual(*packedArray2.GetGeneric<int>(0), expectedElement1);
            
            Assert.AreEqual(*packedArray2.GetGeneric<int>(1), expectedElement2);
            
            Assert.AreEqual(*packedArray2.GetGeneric<int>(2), expectedElement3);
                
                
            //Values behind pointers should be equal to the input
            Assert.AreEqual(*(int*)pointer1, expectedElement1);
            
            Assert.AreEqual(*(int*)pointer2, expectedElement2);
            
            Assert.AreEqual(*(int*)pointer3, expectedElement3);
            
            Assert.AreEqual(*(int*)pointer, expectedElement3);
                
                
            //Value behind generic pointer should be equal to the input
            Assert.AreEqual(*genericPointer1, expectedElement1);
            
            Assert.AreEqual(*genericPointer2, expectedElement2);
            
            Assert.AreEqual(*genericPointer3, expectedElement3);
            
            Assert.AreEqual(*genericPointer, expectedElement3);
                
                
            //Array indexes obtained from pointers should be equal to the input
            Assert.AreEqual(packedArray1.IndexOfPointer(pointer1), 0);
            
            Assert.AreEqual(packedArray1.IndexOfPointer(pointer2), 1);
            
            Assert.AreEqual(packedArray1.IndexOfPointer(pointer3), 2);

			Assert.AreEqual(packedArray1.IndexOfPointer(pointer), 2);
                
                
            //Array indexes obtained from generic pointers should be equal to the input
            Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer1), 0);
            
            Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer2), 1);
            
            Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer3), 2);

			Assert.AreEqual(packedArray2.IndexOfGeneric<int>(genericPointer), 2);
        }
    }
}