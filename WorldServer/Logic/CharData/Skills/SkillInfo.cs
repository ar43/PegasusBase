using WorldServer.Enums;

namespace WorldServer.Logic.CharData.Skills
{
	internal class SkillInfo
	{
		public SkillInfo()
		{
			MainData = new();
		}

		public Dictionary<int, SkillInfoMain> MainData { get; private set; }

		public void Add(int id, SkillInfoMain mainInfo)
		{
			MainData.Add(id, mainInfo);
		}
		
	}

	internal class SkillInfoMain
	{
		
	}

	
}
