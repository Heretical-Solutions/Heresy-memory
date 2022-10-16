using NUnit.Framework;
using HereticalSolutions.Collections.Factories;
using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Allocations;

namespace HereticalSolutions.Collections.Tests
{
	public class IndexedPackedArrayTests
	{
		[TestCase(5, 1, 3, -5)]
		[TestCase(3, 0, -100, 100)]
		public void Push_Values_ValuesStored(
			int arraySize,
			int element1,
			int element2,
			int element3)
		{
				//Allocation
				IndexedPackedArray<int> packedArray = CollectionFactory.BuildIndexedPackedArray<int>(
					CollectionFactory.BuildPoolElementAllocationCommand<int>(
						new AllocationCommand<int>
						{
							Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
							AllocationDelegate = () => { return -1; },
							Amount = arraySize
						},
						CollectionFactory.BuildIndexedContainer));

				//Operation

				//Store elements in packedArray

				var instance1 = packedArray.Pop();

				instance1.Value = element1;


				var instance2 = packedArray.Pop();

				instance2.Value = element2;


				var instance3 = packedArray.Pop();

				instance3.Value = element3;


				//Comparison

				//Count of elements stored should be equal to the input
				Assert.AreEqual(packedArray.Count, 3);


				//If allocated space is more than element count then there should be some free space left
				if (arraySize == 3)
				{
					Assert.IsFalse(packedArray.HasFreeSpace);
				}
				else
				{
					Assert.IsTrue(packedArray.HasFreeSpace);
				}


				//Elements obtained by indexer should be equal to the input
				Assert.AreEqual(packedArray[0].Value, element1);

				Assert.AreEqual(packedArray[1].Value, element2);

				Assert.AreEqual(packedArray[2].Value, element3);


				//Values behind containers should be equal to the input
				Assert.AreEqual(instance1.Value, element1);

				Assert.AreEqual(instance2.Value, element2);

				Assert.AreEqual(instance3.Value, element3);
		}

		[TestCase(3, 1, 3, -5)]
		[TestCase(1, 0, -100, 100)]
		public void Push_Values_ValuesStored_Pop_Values_ValuesErased(
			int arraySize,
			int element1,
			int element2,
			int element3)
		{
				//Allocation
				IndexedPackedArray<int> packedArray = CollectionFactory.BuildIndexedPackedArray<int>(
					CollectionFactory.BuildPoolElementAllocationCommand<int>(
						new AllocationCommand<int>
						{
							Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
							AllocationDelegate = () => { return -1; },
							Amount = arraySize
						},
						CollectionFactory.BuildIndexedContainer));

				//Operation and comparison

				//Push/pop elements from the input and perform assertions
				PushAssertPopAssert(packedArray, arraySize, element1);

				PushAssertPopAssert(packedArray, arraySize, element2);

				PushAssertPopAssert(packedArray, arraySize, element3);
		}

		/// <summary>
		/// Push value into empty packed array, perform assertion, pop value back and perform assertion again
		/// </summary>
		/// <param name="packedArray">Packed array</param>
		/// <param name="arraySize">Array size</param>
		/// <param name="element">Element value</param>
		private void PushAssertPopAssert(
			IndexedPackedArray<int> packedArray,
			int arraySize,
			int element)
		{
			//Store element in packedArray

			var instance = packedArray.Pop();

			instance.Value = element;


			//Count of elements stored should be equal to 1
			Assert.AreEqual(packedArray.Count, 1);


			//If allocated space is more than element count then there should be some free space left
			if (arraySize == 1)
			{
				Assert.IsFalse(packedArray.HasFreeSpace);
			}
			else
			{
				Assert.IsTrue(packedArray.HasFreeSpace);
			}

			//Element obtained by indexer should be equal to the input
			Assert.AreEqual(packedArray[0].Value, element);


			//Value behind container should be equal to the input
			Assert.AreEqual(instance.Value, element);


			//Remove element from packedArray
			packedArray.Push(instance);


			//Count of elements stored should be equal to 0
			Assert.AreEqual(packedArray.Count, 0);


			//There definitely should be some free space left
			Assert.IsTrue(packedArray.HasFreeSpace);
		}

		[TestCase(5, 1, 3, -5)]
		[TestCase(3, 0, -100, 100)]
		public void Push_Values_Pop_Then_Push_Values_ValuesReordered(
			int arraySize,
			int element1,
			int element2,
			int element3)
		{
				//Allocation
				IndexedPackedArray<int> packedArray = CollectionFactory.BuildIndexedPackedArray<int>(
					CollectionFactory.BuildPoolElementAllocationCommand<int>(
						new AllocationCommand<int>
						{
							Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
							AllocationDelegate = () => { return -1; },
							Amount = arraySize
						},
						CollectionFactory.BuildIndexedContainer));

				//Operation and comparison

				//Store elements in packedArray

				var instance1 = packedArray.Pop();

				instance1.Value = element1;


				var instance2 = packedArray.Pop();

				instance2.Value = element2;


				var inistance3 = packedArray.Pop();

				inistance3.Value = element3;


				//Pop/push elements in the arrays and perform assertions
				PopAssertPushAssert(
					packedArray,
					arraySize,

					1,

					instance1,
					instance2,
					inistance3,

					element1,
					element3,
					element2);

				PopAssertPushAssert(
					packedArray,
					arraySize,

					0,

					instance1,
					instance2,
					inistance3,

					element2,
					element3,
					element1);

				PopAssertPushAssert(
					packedArray,
					arraySize,

					2,

					instance1,
					instance2,
					inistance3,

					element2,
					element3,
					element1);
		}

		/// <summary>
		/// Pop element from packed arrays at given index, perform assertions, then push it back and perform assertions again
		/// </summary>
		/// <param name="packedArray">Packed array 1</param>
		/// <param name="arraySize">Array size</param>
		/// <param name="index">Target element index</param>
		/// <param name="expectedElement1">Expected value of element 1 after the push</param>
		/// <param name="expectedElement2">Expected value of element 2 after the push</param>
		/// <param name="expectedElement3">Expected value of element 3 after the push</param>
		private void PopAssertPushAssert(
			IndexedPackedArray<int> packedArray,

			int arraySize,
			int index,

			IPoolElement<int> instance1,
			IPoolElement<int> instance2,
			IPoolElement<int> instance3,

			int expectedElement1,
			int expectedElement2,
			int expectedElement3)
		{
			//Pop elements by target index in packed arrays
			//Return values should be equal to 2 (assuming there were only 3 elements stored)
			packedArray.Push(index);


			//Count of elements stored should be equal to 2 (assuming there were only 3 elements stored)
			Assert.AreEqual(packedArray.Count, 2);


			//There definitely should be some free space left
			Assert.IsTrue(packedArray.HasFreeSpace);


			//Elements obtained by indexer should be equal to the input
			Assert.AreEqual(packedArray[0].Value, expectedElement1);

			Assert.AreEqual(packedArray[1].Value, expectedElement2);


			//Push popped elements back into packed arrays

			var instance = packedArray.Pop();

			instance.Value = expectedElement3;


			//Count of elements stored should be equal to 3 (assuming there were only 3 elements stored)
			Assert.AreEqual(packedArray.Count, 3);


			//If allocated space is more than element count then there should be some free space left
			if (arraySize == 3)
			{
				Assert.IsFalse(packedArray.HasFreeSpace);
			}
			else
			{
				Assert.IsTrue(packedArray.HasFreeSpace);
			}


			//Elements obtained by indexer should be equal to the input
			Assert.AreEqual(packedArray[0].Value, expectedElement1);

			Assert.AreEqual(packedArray[1].Value, expectedElement2);

			Assert.AreEqual(packedArray[2].Value, expectedElement3);
		}
	}
}