using LibPegasus.Enums;
using Npgsql;
using Serilog;
using System.Diagnostics;

namespace MasterServer.DB
{
	public class AccountManager
	{
		private NpgsqlDataSource _dataSource;

		public AccountManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}
		private async Task<(string hash, uint accountId)> GetAccountInfo(string username)
		{
			await using var conn = await _dataSource.OpenConnectionAsync();

			await using var cmd = new NpgsqlCommand("SELECT password, id FROM main.accounts WHERE username=@p", conn);
			cmd.Parameters.AddWithValue("p", username);

			await using var reader = await cmd.ExecuteReaderAsync();
			if (await reader.ReadAsync())
			{
				var hash = reader.GetString(0);
				var id = reader.GetInt32(1);
				Debug.Assert(hash != String.Empty);
				return (hash, (uint)id);
			}

			return (String.Empty, 0);
		}
		public async Task<InfoCodeLS> RequestRegister(string username, string password)
		{
			var passwordHash = await Task.Run(() =>
				BCrypt.Net.BCrypt.HashPassword(password));

			try
			{
				await using var conn = await _dataSource.OpenConnectionAsync();

				await using var cmd = new NpgsqlCommand("INSERT INTO main.accounts (id, username, password) VALUES (DEFAULT, @username, @password)", conn);

				cmd.Parameters.AddWithValue("username", username);
				cmd.Parameters.AddWithValue("password", passwordHash);

				await cmd.ExecuteNonQueryAsync();

				Log.Information("Registered account with username {Username}", username);
				return InfoCodeLS.REGISTRATION_OK;
			}
			catch (PostgresException ex) when (ex.SqlState == "23505") //unique violation
			{
				return InfoCodeLS.REGISTRATION_USEREXISTS;
			}
		}
		public async Task<UInt32> RequestLogin(string username, string password)
		{
			var accountInfo = await GetAccountInfo(username);

			if (string.IsNullOrEmpty(accountInfo.hash))
			{
				return 0;
			}

			// Offload CPU-heavy BCrypt hashing properly without thread blocking
			bool isValid = await Task.Run(() => BCrypt.Net.BCrypt.Verify(password, accountInfo.hash));

			if (isValid)
			{
				return accountInfo.accountId;
			}

			return 0;
		}

	}
}
