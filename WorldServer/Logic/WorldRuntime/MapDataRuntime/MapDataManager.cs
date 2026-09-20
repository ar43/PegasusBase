using LibPegasus.Utils;
using System.Globalization;
using System.Text.RegularExpressions;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapDataManager
	{
		Dictionary<int, MapData> _maps;
		WorldConfig _config;
		MobDataManager _mobDataManager;

		public MapDataManager(WorldConfig config, MobDataManager mobDataManager)
		{
			_maps = new();
			_config = config;
			_mobDataManager = mobDataManager;
		}

		public MapData Get(int mapId)
		{
			if (_maps.TryGetValue(mapId, out var map))
			{
				return map;
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
