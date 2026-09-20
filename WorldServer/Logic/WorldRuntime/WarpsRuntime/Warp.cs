namespace WorldServer.Logic.WorldRuntime.WarpsRuntime
{
	internal class Warp
	{
		public Warp(int warpId, Int32 worldIdx, Int32 posXPnt, Int32 posYPnt)
		{
			WarpId = warpId;
			WorldIdx = worldIdx;
			PosXPnt = posXPnt;
			PosYPnt = posYPnt;
		}

		public int WarpId { get; private set; }
		public int WorldIdx { get; private set; }
		public int PosXPnt { get; private set; }
		public int PosYPnt { get; private set; }
	}
}
