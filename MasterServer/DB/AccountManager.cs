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
		private async Task<bool> AccountExists(string username)
		{
			using var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM main.accounts WHERE username=@p)", conn))
			{
				cmd.Parameters.AddWithValue("p", username);
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					bool found = await reader.ReadAsync();
					if (found)
					{
						bool output = reader.GetBoolean(0);
						return output;
					}
					else
					{
						return false;
					}
				}
			}
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
		private async Task<bool> RegisterAccount(string username, string password)
		{
			var hashPassword = Task.Factory.StartNew(() =>
			{
				string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
				return passwordHash;
			});

			using var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("INSERT INTO main.accounts VALUES (DEFAULT, @u, @p)", conn))
			{
				cmd.Parameters.AddWithValue("u", username);
				cmd.Parameters.AddWithValue("p", hashPassword.Result);
				await cmd.ExecuteNonQueryAsync();
				return true;
			}
		}
		public async Task<InfoCodeLS> RequestRegister(string username, string password)
		{
			// TODO

			bool exists = await AccountExists(username);

			if (exists)
			{
				return InfoCodeLS.REGISTRATION_USEREXISTS;
			}
			else
			{
				bool accountRegistered = await RegisterAccount(username, password);

				if (accountRegistered)
				{
					Log.Information($"Registered account with username {username}");
					return InfoCodeLS.REGISTRATION_OK;
				}
				else
				{
					return InfoCodeLS.REGISTRATION_FAILED;
				}
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
