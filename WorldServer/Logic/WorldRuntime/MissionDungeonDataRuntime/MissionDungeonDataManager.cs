using LibPegasus.Utils;
using System.Diagnostics;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;

namespace WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime
{
	internal class MissionDungeonDataManager
	{
		private readonly WorldConfig _config;
		private readonly MobDataManager _mobDataManager;
		public MissionDungeonDataManager(WorldConfig worldConfig, MobDataManager mobDataManager)
		{
			MainData = new();
			_config = worldConfig;
			_mobDataManager = mobDataManager;
			LoadConfig();
		}

		public Dictionary<int, MissionDungeonDataMain> MainData { get; private set; }

		private void LoadConfig()
		{
			MainData = new();
		}
	}

	internal class MissionDungeonDataMain
	{
		public MissionDungeonDataMain(MissionDungeonInfo missionDungeonInfo)
		{
			MissionDungeonInfo = missionDungeonInfo;
			MissionDungeonMMap = new();
			MissionDungeonTriggers = new();
			MissionDungeonActGroup = new();
		}

		public MissionDungeonInfo MissionDungeonInfo { get; private set; }
		public Dictionary<int, MissionDungeonMMapEntry> MissionDungeonMMap { get; private set; }
		public Dictionary<int, MissionDungeonTrigger> MissionDungeonTriggers { get; private set; }
		public MissionDungeonPP? MissionDungeonPP { get; set; }
		public List<MissionDungeonActGroup> MissionDungeonActGroup { get; private set; }

	}

	internal class MissionDungeonInfo
	{
		public MissionDungeonInfo(Int32 qDungeonIdx, Int32 instanceLimit, Int32 level, Int32 maxUser, Int32 missionTimeout, Int32[]? openItem, Int32 pPLink, Int32 penalty, Int32[]? reward, Int32 warpIdx, Int32 warpIdxForSucess, Int32 warpIdxForFail, Int32 warpIndexForDead, Int32 worldIdx, Int32 bAddRange, Int32 nextQDIdxforSuccess, Int32 warpNPC_Set, Int32 useTerrain, Int32[]? battleStyle, Int32 useOddCircle_Count, Int32 party_Type, Int32 removeItem, Int32 dBWrite, Int32 dungeonType)
		{
			QDungeonIdx = qDungeonIdx;
			InstanceLimit = instanceLimit;
			Level = level;
			MaxUser = maxUser;
			MissionTimeout = missionTimeout;
			OpenItem = openItem;
			PPLink = pPLink;
			Penalty = penalty;
			Reward = reward;
			WarpIdx = warpIdx;
			WarpIdxForSucess = warpIdxForSucess;
			WarpIdxForFail = warpIdxForFail;
			WarpIndexForDead = warpIndexForDead;
			WorldIdx = worldIdx;
			this.bAddRange = bAddRange;
			NextQDIdxforSuccess = nextQDIdxforSuccess;
			WarpNPC_Set = warpNPC_Set;
			UseTerrain = useTerrain;
			BattleStyle = battleStyle;
			UseOddCircle_Count = useOddCircle_Count;
			Party_Type = party_Type;
			RemoveItem = removeItem;
			DBWrite = dBWrite;
			DungeonType = dungeonType;
		}

		public int QDungeonIdx { get; private set; }
		public int InstanceLimit { get; private set; }
		public int Level { get; private set; }
		public int MaxUser { get; private set; }
		public int MissionTimeout { get; private set; }
		public int[]? OpenItem { get; private set; }
		public int PPLink { get; private set; }
		public int Penalty { get; private set; }
		public int[]? Reward { get; private set; }
		public int WarpIdx { get; private set; }
		public int WarpIdxForSucess { get; private set; }
		public int WarpIdxForFail { get; private set; }
		public int WarpIndexForDead { get; private set; }
		public int WorldIdx { get; private set; }
		public int bAddRange { get; private set; }
		public int NextQDIdxforSuccess { get; private set; }
		public int WarpNPC_Set { get; private set; }
		public int UseTerrain { get; private set; }
		public int[]? BattleStyle { get; private set; }
		public int UseOddCircle_Count { get; private set; }
		public int Party_Type { get; private set; }
		public int RemoveItem { get; private set; }
		public int DBWrite { get; private set; }
		public int DungeonType { get; private set; }
	}

	internal class MissionDungeonMMapEntry
	{
		public MissionDungeonMMapEntry()
		{

		}

		//public MobSpawnData MobSpawnData { get; private set; }
		//public ExtraMobInfo ExtraMobInfo { get; private set; }

	}

	internal class ExtraMobInfo
	{
		public ExtraMobInfo(Int32 mobIdx, Int32 pPIdx, Int32 spwnCount, Int32 grade, Int32 lv, Int32 trgIdxSpawn, Int32 trgIdxKill)
		{
			MobIdx = mobIdx;
			PPIdx = pPIdx;
			SpwnCount = spwnCount;
			Grade = grade;
			Lv = lv;
			TrgIdxSpawn = trgIdxSpawn;
			TrgIdxKill = trgIdxKill;
		}

		public int MobIdx { get; private set; }
		public int PPIdx { get; private set; }
		public int SpwnCount { get; private set; }
		public int Grade { get; private set; }
		public int Lv { get; private set; }
		public int TrgIdxSpawn { get; private set; }
		public int TrgIdxKill { get; private set; }
	}

	internal class MissionDungeonTrigger
	{
		public MissionDungeonTrigger(Int32 qDungeonIdx, Int32 trgIdx, Int32 order, Int32 trgType, String? liveStateMMapIdx, String? deadStateMMapIdx, Int32 trgNpcIdx, Int32 evtActGroupIdx)
		{
			QDungeonIdx = qDungeonIdx;
			TrgIdx = trgIdx;
			Order = order;
			TrgType = trgType;
			LiveStateMMapIdx = liveStateMMapIdx;
			DeadStateMMapIdx = deadStateMMapIdx;
			TrgNpcIdx = trgNpcIdx;
			EvtActGroupIdx = evtActGroupIdx;
		}

		public int QDungeonIdx { get; private set; }
		public int TrgIdx { get; private set; }
		public int Order { get; private set; }
		public int TrgType { get; private set; }
		public string? LiveStateMMapIdx { get; private set; }
		public string? DeadStateMMapIdx { get; private set; }
		public int TrgNpcIdx { get; private set; }
		public int EvtActGroupIdx { get; private set; }
	}

	internal class MissionDungeonPP
	{
		public MissionDungeonPP(Int32 pPIdx, Int32[]? missionMobs, Int32 missionNPC)
		{
			PPIdx = pPIdx;
			MissionMobs = missionMobs;
			MissionNPC = missionNPC;
		}

		public int PPIdx { get; private set; }
		public int[]? MissionMobs { get; private set; }
		public int MissionNPC { get; private set; }
	}

	internal class MissionDungeonActGroup
	{
		public MissionDungeonActGroup(Int32 qDungeonIdx, Int32 evtActGroupIdx, Int32 order, Int32 tgtMMapIdx, Int32 tgtAction, Int32 evtDelay, string tgtSpawnInterval, string tgtSpawnCount)
		{
			QDungeonIdx = qDungeonIdx;
			EvtActGroupIdx = evtActGroupIdx;
			Order = order;
			TgtMMapIdx = tgtMMapIdx;
			TgtAction = tgtAction;
			EvtDelay = evtDelay;
			TgtSpawnInterval = tgtSpawnInterval;
			TgtSpawnCount = tgtSpawnCount;
		}

		public int QDungeonIdx { get; private set; }
		public int EvtActGroupIdx { get; private set; }
		public int Order { get; private set; }
		public int TgtMMapIdx { get; private set; }
		public int TgtAction { get; private set; }
		public int EvtDelay { get; private set; }
		public string TgtSpawnInterval { get; private set; }
		public string TgtSpawnCount { get; private set; }
	}
}
