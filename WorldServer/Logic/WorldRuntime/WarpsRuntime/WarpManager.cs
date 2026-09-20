namespace WorldServer.Logic.WorldRuntime.WarpsRuntime
{
	internal class WarpManager
	{
		private Dictionary<int, Warp> _warps;

		public WarpManager(WorldConfig config)
		{
			_warps = new();
		}

		public Warp? Get(int warpId)
		{
			if (_warps.TryGetValue(warpId, out var warp))
			{
				return warp;
			}
			else
			{
				return null;
			}
		}
	}
}
