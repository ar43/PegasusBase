using WorldServer.Logic.WorldRuntime.MobDataRuntime;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MobSpawnData
	{
		public MobSpawnData(Int32 speciesIdx, MobData mobData, Int32 posX, Int32 posY, Int32 width, Int32 height)
		{
			SpeciesIdx = speciesIdx;
			MobData = mobData;
			PosX = posX;
			PosY = posY;
			Width = width;
			Height = height;
		}

		public int SpeciesIdx { get; private set; }
		public MobData MobData { get; private set; }
		public int PosX { get; private set; }
		public int PosY { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
	}
}
