using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HereticalSolutions.Memory;
using System.Runtime.InteropServices;
using System;

namespace HereticalSolutions.Memory.Tests
{
    public class UnmanagedArrayTests
    {
        [TestCase(5, 0, 1, 2, 3, 4, -5)]
        [TestCase(3, 0, 0, 1, -100, 2, 100)]
        public void Add_Values_ValuesPresent(
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
                *(int*)(unmanagedArray[index1]) = element1;
                
                *(int*)(unmanagedArray[index2]) = element2;
                
                *(int*)(unmanagedArray[index3]) = element3;
                
                //Comparison
                Assert.AreEqual(*(int*)(unmanagedArray[index1]), element1);
                
                Assert.AreEqual(*(int*)(unmanagedArray[index2]), element2);
                
                Assert.AreEqual(*(int*)(unmanagedArray[index3]), element3);
                
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
                *(int*)(unmanagedArray[index]) = element1;
                
                Assert.AreEqual(*(int*)(unmanagedArray[index]), element1);
                
                *(int*)(unmanagedArray[index]) = element2;
                
                Assert.AreEqual(*(int*)(unmanagedArray[index]), element2);
                
                *(int*)(unmanagedArray[index]) = element3;
                
                Assert.AreEqual(*(int*)(unmanagedArray[index]), element3);
                
                //Deallocation
                Marshal.FreeHGlobal(pointer);
            }
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