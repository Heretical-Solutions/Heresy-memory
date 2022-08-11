using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HereticalSolutions.Memory;
using System.Runtime.InteropServices;

namespace HereticalSolutions.Memory.Tests
{
    public class UnmanagedGenericArrayTests
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
                
                var pointer = (int*)Marshal.AllocHGlobal(memorySize);
                
                UnmanagedGenericArray<int> unmanagedArray = new UnmanagedGenericArray<int>(
                    pointer,
                    memorySize,
                    elementSize,
                    elementCapacity);
                
                //Operation
                unmanagedArray[index1] = element1;
                
                unmanagedArray[index2] = element2;
                
                unmanagedArray[index3] = element3;
                
                //Comparison
                Assert.AreEqual(unmanagedArray[index1], element1);
                
                Assert.AreEqual(unmanagedArray[index2], element2);
                
                Assert.AreEqual(unmanagedArray[index3], element3);
                
                //Deallocation
                Marshal.FreeHGlobal((IntPtr)pointer);
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
                    
                var pointer = (int*)Marshal.AllocHGlobal(memorySize);
                    
                UnmanagedGenericArray<int> unmanagedArray = new UnmanagedGenericArray<int>(
                    pointer,
                    memorySize,
                    elementSize,
                    elementCapacity);
                    
                //Operation and comparison
                unmanagedArray[index] = element1;
                    
                Assert.AreEqual(unmanagedArray[index], element1);
                    
                unmanagedArray[index] = element2;
                    
                Assert.AreEqual(unmanagedArray[index], element2);
                    
                unmanagedArray[index] = element3;
                    
                Assert.AreEqual(unmanagedArray[index], element3);
                    
                //Deallocation
                Marshal.FreeHGlobal((IntPtr)pointer);
            }
        }
    }
}
