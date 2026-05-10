using LibPegasus.Crypt;
using LibPegasus.Enums;
using LibPegasus.Utils;
using LibPegasus.Packets.Login.S2C;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Google.Protobuf;

namespace LoginServer.Logic.Delegates
{
	internal static class Connection
	{
		public static void OnServerConnection(Client client, byte[] clientNonce, byte[] clientPublicKey)
		{
			Serilog.Log.Debug("OnServerConnection called");

			if (client.ClientInfo.ConnState != Enums.ConnState.INITIAL)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			client.ClientInfo.ConnState = Enums.ConnState.CONNECTED;

			client.Encryption.GenerateSessionKey(client.ClientInfo.ServerNonce, clientNonce, clientPublicKey);

			var packet = new RSP_Connect2Svr<Client>(client.ClientInfo.AuthKey, client.ClientInfo.UserId, 
				client.ClientInfo.ServerNonce, client.Encryption.KeyPair.PublicKey);
			client.PacketManager.Send(packet);
		}

		public static void OnCheckVersion(Client client, UInt32 clientVersion)
		{
			var serverConfig = ServerConfig.Get();
			var expectedVersion = LibPegasus.Packets.Login.LoginPacketVersion.Revision;

			if (client.ClientInfo.ConnState != Enums.ConnState.CONNECTED && client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			if (clientVersion != expectedVersion && serverConfig.GeneralSettings.VerifyClientVersion)
			{
				Serilog.Log.Debug($"OnCheckVersion: received version from client: {clientVersion}, expected {expectedVersion}");
				client.Disconnect("invalid version");
				return;
			}

			if (client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT)
				client.ClientInfo.ConnState = Enums.ConnState.VERSION_CHECKED;

			Serilog.Log.Debug($"OnCheckVersion: received version from client: {clientVersion}");

			var packet = new RSP_CheckVersion<Client>((uint)expectedVersion);
			client.PacketManager.Send(packet);
		}

		public static async void OnAuthAccount(Client client, byte usernameLen, string username, byte passwordLen, string password)
		{
			if (client.ClientInfo.ConnState != Enums.ConnState.VERSION_CHECKED)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			//Serilog.Log.Debug($"username extracted: {username} (len: {username.Length})");
			//Serilog.Log.Debug($"password extracted: {password} (len: {password.Length})");

			var reply = await client.SendLoginRequest(username, password);
			if ((AuthResult)reply.Status == AuthResult.Normal)
			{
				bool isLocalhost = client.Ip == "127.0.0.1";
				Debug.Assert(reply.AuthKey.Length == 32);
				var replyServerState = await client.GetServerState(isLocalhost);

				var loginAccountReplyBytes = reply.ToByteArray();
				var serverStateReplyBytes = replyServerState.ToByteArray();

				var packetServerState = new NFY_ServerState<Client>(serverStateReplyBytes);
				client.PacketManager.Send(packetServerState);

				var packetAuth = new RSP_AuthAccount<Client>(loginAccountReplyBytes);
				client.PacketManager.Send(packetAuth);

				client.ClientInfo.ConnState = Enums.ConnState.AUTH_ACCOUNT;
				client.ClientInfo.AccountId = reply.AccountId;

				Serilog.Log.Debug($"{username} logged in");
			}
			else
			{
				var packet = new RSP_AuthAccount<Client>(reply.ToByteArray());
				client.PacketManager.Send(packet);
				client.Disconnect("bad auth");
			}
		}

		internal static async void OnVerifyLinks(Client client, UInt32 authKey, UInt16 userId, Byte channelId, Byte serverId, UInt32 clientMagicKey)
		{
			var cfg = ServerConfig.Get();
			if (client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT || client.ClientInfo.AccountId == 0)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			client.ClientInfo.ConnState = Enums.ConnState.VERIFYING;
			//TODO: check if authKey expired (5 sec?)
			var reply = await client.SendSessionRequest(authKey, userId, channelId, serverId);
			bool success = reply.Result == (uint)SessionResult.OK || reply.Result == (uint)SessionResult.REPLACED;
			var packet = new RSP_VerifyLinks<Client>(channelId, serverId, success);
			client.PacketManager.Send(packet);

			if (success)
			{
				client.ClientInfo.ConnState = Enums.ConnState.VERIFIED;
				client.Disconnect("Linked - success");

				//TODO: disconnect??
			}
			else
			{
				client.Disconnect("Linked - fail");
			}

		}
	}
}
