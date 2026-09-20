using LibPegasus.Parsers.Scp;
using System.Text.RegularExpressions;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.CharData.Quests;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.Extra;

namespace WorldServer.Logic.WorldRuntime
{
	internal class WorldConfig
	{

		public WorldConfig()
		{
			//_config = [];
			string workingDirectory = Environment.CurrentDirectory;
			string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
			/*
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Warp.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\NPCShop.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Item.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Mobs.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Skill.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Rank.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Quest.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\ItemReward.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Level.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\MissionDungeon.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\World_drop.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\OptionPool.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_Custom\\QuestToDungeon.scp");

			Item.LoadConfigs(this);
			Item.LoadItemRewards(this);
			Skill.LoadConfigs(this);
			Style.LoadConfigs(this);
			Quest.LoadConfigs(this);
			Loot.LoadConfig(this);
			Stats.LoadExpTable(this);

			*/

			CommandManager.Init();
		}

		//private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _config;

		
	}
}
