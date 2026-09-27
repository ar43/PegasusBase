using LibPegasus.Enums;
using Npgsql;

namespace MasterServer.DB
{
	public class SessionManager
	{
		private NpgsqlDataSource _dataSource;

		public SessionManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}
		public async Task<SessionResult> Create(uint authKey, ushort userId, byte channelId, byte serverId, uint accountId)
		{
			await using var conn = await _dataSource.OpenConnectionAsync();
			await using var tx = await conn.BeginTransactionAsync();

			SessionResult result = SessionResult.OK;

			await using (var delete = new NpgsqlCommand("""
				DELETE FROM main.sessions
				WHERE account_id = @accountId
				RETURNING 1
				""", conn, tx))
			{
				delete.Parameters.AddWithValue("accountId", (long)accountId);

				var deleted = await delete.ExecuteScalarAsync();

				if (deleted != null)
					result = SessionResult.REPLACED;
			}

			await using (var insert = new NpgsqlCommand("""
				INSERT INTO main.sessions
					(auth_key, user_id, channel_id, server_id, account_id)
				VALUES
					(@authKey, @userId, @channelId, @serverId, @accountId)
				""", conn, tx))
			{
				insert.Parameters.AddWithValue("authKey", (long)authKey);
				insert.Parameters.AddWithValue("userId", (int)userId);
				insert.Parameters.AddWithValue("channelId", (int)channelId);
				insert.Parameters.AddWithValue("serverId", (int)serverId);
				insert.Parameters.AddWithValue("accountId", (long)accountId);

				await insert.ExecuteNonQueryAsync();
			}

			await tx.CommitAsync();

			return result;
		}
	}
}
