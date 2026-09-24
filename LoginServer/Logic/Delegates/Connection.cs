using LibPegasus.Crypt;
using LibPegasus.Enums;
using LibPegasus.Utils;
using LibPegasus.Packets.Login.S2C;
using LibPegasus.Protobuf.Login;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Google.Protobuf;

namespace LoginServer.Logic.Delegates
{
	internal static class Connection
	{
		public static void ConnectServerHandler(Client client, ConnectServerReq req)
		{
			Serilog.Log.Debug("OnServerConnection called");

			if (client.ClientInfo.ConnState != Enums.ConnState.INITIAL)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			client.ClientInfo.ConnState = Enums.ConnState.CONNECTED;

			client.Encryption.GenerateSessionKey(client.ClientInfo.ServerNonce, req.ClientNonce.ToByteArray(), req.ClientPublicKey.ToByteArray());

			var packet = new RSP_ConnectServer<Client>(new ConnectServerRsp
			{
				AuthKey = client.ClientInfo.AuthKey,
				UserIdx = client.ClientInfo.UserId,
				ServerNonce = ByteString.CopyFrom(client.ClientInfo.ServerNonce),
				PublicServerKey = ByteString.CopyFrom(client.Encryption.KeyPair.PublicKey)
			});
			client.PacketManager.Send(packet);
		}

		public static void CheckVersionHandler(Client client, CheckVersionReq req)
		{
			var serverConfig = ServerConfig.Get();
			var expectedVersion = LibPegasus.Packets.Login.LoginPacketVersion.Revision;

			if (client.ClientInfo.ConnState != Enums.ConnState.CONNECTED && client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			if (req.ClientVersion != expectedVersion && serverConfig.GeneralSettings.VerifyClientVersion)
			{
				var packetFail = new RSP_CheckVersion<Client>(new CheckVersionRsp
				{
					VersionOk = 0
				});
				client.PacketManager.Send(packetFail);
				client.Disconnect("invalid version");
				return;
			}

			if (client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT)
				client.ClientInfo.ConnState = Enums.ConnState.VERSION_CHECKED;

			Serilog.Log.Debug($"OnCheckVersion: received version from client: {req.ClientVersion}");

			var packet = new RSP_CheckVersion<Client>(new CheckVersionRsp
			{
				VersionOk = 1
			});
			client.PacketManager.Send(packet);
		}

		public static async void AuthAccountHandler(Client client, AuthAccountReq req)
		{
			if (client.ClientInfo.ConnState != Enums.ConnState.VERSION_CHECKED)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			Serilog.Log.Debug($"username extracted: {req.Username}");
			Serilog.Log.Debug($"password extracted: {req.Password}");

			var reply = await client.SendLoginRequest(req.Username, req.Password);
			if ((AuthResult)reply.Status == AuthResult.SUCCESS)
			{
				bool isLocalhost = client.Ip == "127.0.0.1";
				var replyServerState = await client.GetServerState(isLocalhost);

				var loginAccountReplyBytes = reply.ToByteArray();
				var serverStateReplyBytes = replyServerState.ToByteArray();

				var packetServerState = new NFY_ServerState<Client>(new ServerStateNfy 
				{ 
					ServerStateReply = ByteString.CopyFrom(serverStateReplyBytes)
				});
				client.PacketManager.Send(packetServerState);

				var packetAuth = new RSP_AuthAccount<Client>(new AuthAccountRsp
				{
					AuthAccountReply = ByteString.CopyFrom(loginAccountReplyBytes)
				});
				client.PacketManager.Send(packetAuth);

				client.ClientInfo.ConnState = Enums.ConnState.AUTH_ACCOUNT;
				client.ClientInfo.AccountId = reply.AccountId;

				Serilog.Log.Debug($"{req.Username} logged in");
			}
			else
			{
				var packet = new RSP_AuthAccount<Client>(new AuthAccountRsp
				{
					AuthAccountReply = ByteString.CopyFrom(reply.ToByteArray())
				});
				client.PacketManager.Send(packet);
				client.Disconnect($"{req.Username} bad auth");
			}
		}

		internal static async void VerifyLinksHandler(Client client, UInt32 authKey, UInt16 userId, Byte channelId, Byte serverId, UInt32 clientMagicKey)
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
