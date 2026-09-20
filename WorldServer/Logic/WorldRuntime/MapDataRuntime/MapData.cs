namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapData
	{
		public MapData(Int32 mapId, Dictionary<Int32, NpcData> npcData, Dictionary<Int32, MobSpawnData> mobSpawnData)
		{
			MapId = mapId;
			NpcData = npcData;
			MobSpawnData = mobSpawnData;
		}

		public int MapId { get; private set; }
		public Dictionary<int, NpcData> NpcData { get; private set; }
		public Dictionary<int, MobSpawnData> MobSpawnData { get; private set; }
	}
}
