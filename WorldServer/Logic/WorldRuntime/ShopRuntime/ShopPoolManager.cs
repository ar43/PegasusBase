namespace WorldServer.Logic.WorldRuntime.ShopRuntime
{
	internal class ShopPoolManager
	{
		Dictionary<int, ShopPool> _poolCollection;
		public ShopPoolManager(WorldConfig worldConfig)
		{
			_poolCollection = new Dictionary<int, ShopPool>();
		}

		public ShopPool GetPool(int poolId)
		{
			if (!_poolCollection.TryGetValue(poolId, out var pool))
			{
				throw new Exception("undefined pool");
			}
			return pool;
		}

		public void CreatePool(ShopPool newPool)
		{
			if (_poolCollection.ContainsKey(newPool.PoolId))
				throw new Exception("pool already exists");
			_poolCollection[newPool.PoolId] = newPool;
		}

		public int Count()
		{
			return _poolCollection.Count;
		}
	}
}
