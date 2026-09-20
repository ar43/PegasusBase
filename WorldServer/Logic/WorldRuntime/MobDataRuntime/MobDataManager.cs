using System.Globalization;

namespace WorldServer.Logic.WorldRuntime.MobDataRuntime
{
	internal class MobDataManager
	{
		private Dictionary<int, MobData> _mobData;

		public MobDataManager(WorldConfig _worldConfig)
		{
			_mobData = new Dictionary<int, MobData>();
		}

		public MobData Get(int id)
		{
			return _mobData[id];
		}
	}
}
