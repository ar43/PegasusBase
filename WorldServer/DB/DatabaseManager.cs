using Npgsql;

namespace WorldServer.DB
{
	internal class DatabaseManager
	{
		public NpgsqlDataSource DataSourceWorld { private set; get; }

		public CharacterManager CharacterManager { private set; get; }

		public DatabaseManager()
		{
			var cfg = ServerConfig.Get();
			var dataSourceBuilderWorld = new NpgsqlDataSourceBuilder(cfg.DatabaseSettings.ConnString);
			DataSourceWorld = dataSourceBuilderWorld.Build();
			CharacterManager = new(DataSourceWorld);
		}
	}
}
