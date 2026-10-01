using Npgsql;

namespace MasterServer.DB
{
	public class DatabaseManager
	{
		private NpgsqlDataSource _dataSource;

		public AccountManager AccountManager { private set; get; }

		public CharacterManager CharacterManager { private set; get; }

		public SessionManager SessionManager { private set; get; }

		//TODO: add other managers

		public DatabaseManager(IConfiguration configuration)
		{
			var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration["ConnString"]);
			_dataSource = dataSourceBuilder.Build();

			AccountManager = new(_dataSource);
			SessionManager = new(_dataSource);
			CharacterManager = new(_dataSource);
		}
	}
}
