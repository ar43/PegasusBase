using LibPegasus.Utils;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime;

namespace WorldServer.Logic.CharData.Quests
{
	internal class Quest
	{
		public Quest(UInt16 id)
		{
			/*
			if (_questConfig == null)
				throw new Exception("Quest Data not yet loaded");
			Id = id;
			QuestInfoMain = _questConfig.MainData[id];
			*/
		}

		/*
		public Quest(UInt16 id, Boolean started, UInt16 flags, UInt32 actCounter, List<byte>? questProgress) : this(id)
		{
			Started = started;
			Flags = flags;
			ActCounter = actCounter;
			MobProgress = null;
			ItemProgress = null;
			if (questProgress?.Count > 0)
			{
				int mobLen = QuestInfoMain.MissionMob == null ? 0 : QuestInfoMain.MissionMob.Length / 2;
				int itemLen = QuestInfoMain.MissionItem == null ? 0 : QuestInfoMain.MissionItem.Length;
				int dungeonLen = QuestInfoMain.MissionDungeon == null ? 0 : QuestInfoMain.MissionDungeon.Length;
				if (mobLen > 0)
				{
					MobProgress = questProgress.Slice(0, mobLen);
				}
				if (itemLen > 0)
				{
					ItemProgress = questProgress.Slice(mobLen, itemLen);
				}
				if (dungeonLen > 0)
				{
					DungeonProgress = questProgress.Slice(itemLen + mobLen, dungeonLen);
				}
			}

		}
		*/
	}
}
