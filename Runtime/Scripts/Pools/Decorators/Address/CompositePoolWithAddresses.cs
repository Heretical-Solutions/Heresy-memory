using System;
using HereticalSolutions.Collections;
using HereticalSolutions.Repositories;
using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools
{
	public class CompositePoolWithAddresses<T>
		: INonAllocDecoratedPool<T>
	{
		private int level;

		private IRepository<int, INonAllocDecoratedPool<T>> poolsRepository;

		public CompositePoolWithAddresses(
			IRepository<int, INonAllocDecoratedPool<T>> poolsRepository,
			int level)
		{
			this.poolsRepository = poolsRepository;

			this.level = level;
		}

		#region Pop

		public IPoolElement<T> Pop(IPoolDecoratorArgument[] args)
		{
			if (!args.TryGetArgument<AddressArgument>(out var arg))
				throw new Exception("[CompositePoolWithAddresses] ADDRESS ARGUMENT ABSENT");

			if (arg.AddressHashes.Length <= level)
				throw new Exception($"[CompositePoolWithAddresses] INVALID ADDRESS DEPTH. LEVEL: {{ {level} }} ADDRESS LENGTH: {{ {arg.AddressHashes.Length} }}");

			int currentAddressHash = arg.AddressHashes[level];

			if (!poolsRepository.TryGet(currentAddressHash, out var pool))
				throw new Exception($"[CompositePoolWithAddresses] INVALID ADDRESS {{ {currentAddressHash} }}");

			var result = pool.Pop(args);

			return result;
		}

		#endregion

		#region Push

		public void Push(
			IPoolElement<T> instance,
			bool dryRun = false)
		{
			var elementWithAddress = (IAddressContainable)instance;

			if (elementWithAddress == null)
				throw new Exception("[CompositePoolWithAddresses] INVALID INSTANCE");

			int currentAddressHash = elementWithAddress.AddressHashes[level];

			if (!poolsRepository.TryGet(currentAddressHash, out var pool))
				throw new Exception($"[CompositePoolWithAddresses] INVALID ADDRESS {{ {currentAddressHash} }}");

			pool.Push(
				instance,
				dryRun);
		}

		#endregion
	}
}