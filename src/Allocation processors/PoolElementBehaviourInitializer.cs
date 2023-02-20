using UnityEngine;
using HereticalSolutions.Collections;
using HereticalSolutions.Pools.Behaviours;

namespace HereticalSolutions.Pools.AllocationProcessors
{
	public class PoolElementBehaviourInitializer : IAllocationProcessor
	{
		public void Process(
			INonAllocDecoratedPool<GameObject> poolWrapper,
			IPoolElement<GameObject> currentElement)
		{
			if (currentElement.Value == null)
				return;

			var behaviour = currentElement.Value.GetComponent<PoolElementBehaviour>();

			if (behaviour == null)
				return;

			if (!behaviour.Initialized)
				behaviour.Initialize(poolWrapper, currentElement);
		}
	}
}