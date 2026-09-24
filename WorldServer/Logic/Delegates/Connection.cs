using Google.Protobuf;
using LibPegasus.Crypt;
using LibPegasus.Enums;
using LibPegasus.Packets.World;
using LibPegasus.Packets.World.S2C;
using LibPegasus.Protobuf.World;
using WorldServer.Enums;

namespace WorldServer.Logic.Delegates
{
	internal static class Connection
	{
		internal static void ConnectServerHandler(Client client, ConnectServerReq req)
		{
			var cfg = ServerConfig.Get();
			if (client.ConnectionInfo.ConnState != ConnState.INITIAL)
			{
				client.Disconnect("invalid handshake", ConnState.ERROR);
				return;
			}

			client.Encryption.GenerateSessionKey(client.ConnectionInfo.ServerNonce, 
				req.ClientNonce.ToByteArray(), req.ClientPublicKey.ToByteArray());

			if (req.ClientVersion != WorldPacketVersion.Revision)
			{
				var packetFail = new RSP_ConnectServer<Client>(new ConnectServerRsp
				{
					ConnectionResult = (Byte)ConnectionResult.VERSION_MISMATCH,
					AuthKey = client.ConnectionInfo.AuthKey,
					UserIdx = 0,
					ServerNonce = ByteString.CopyFrom(client.ConnectionInfo.ServerNonce),
					PublicServerKey = ByteString.CopyFrom(client.Encryption.KeyPair.PublicKey)
				});
				client.PacketManager.Send(packetFail);
				client.Disconnect($"version mismatch, client: {req.ClientVersion} server: {WorldPacketVersion.Revision}", ConnState.ERROR);
				return;
			}

			client.ConnectionInfo.ConnState = Enums.ConnState.AWAITING;

			var packet = new RSP_ConnectServer<Client>(new ConnectServerRsp
			{
				ConnectionResult = (Byte)ConnectionResult.SUCCESS,
				AuthKey = client.ConnectionInfo.AuthKey,
				UserIdx = client.ConnectionInfo.UserId,
				ServerNonce = ByteString.CopyFrom(client.ConnectionInfo.ServerNonce),
				PublicServerKey = ByteString.CopyFrom(client.Encryption.KeyPair.PublicKey)
			});
			client.PacketManager.Send(packet);
		}

		//internal async static void OnVerifyLinks(Client client, UInt32 authKey, UInt16 userId, Byte channelId, Byte serverId, UInt32 clientMagicKey)
		//{
		//	var cfg = ServerConfig.Get();
		//	//TODO: FIX THIS! STUDY HOW THIS WORKS! IT CHANGES!
		//	//if (clientMagicKey != cfg.GeneralSettings.ClientMagicKey)
		//	//{
		//	//	//TODO: Close connection
		//	//	throw new NotImplementedException();
		//	//}

		//	client.ConnectionInfo.ConnState = ConnState.AWAITING_LINK_REPLY;
		//	//TODO: check if authKey expired (5 sec?)
		//	var reply = await client.SendLoginSessionRequest(authKey, userId, channelId, serverId);
		//	bool success = reply.Result == (uint)SessionResult.OK || reply.Result == (uint)SessionResult.REPLACED;
		//	var packet = new RSP_VerifyLinks(channelId, serverId, success);
		//	client.PacketManager.Send(packet);

		//	if (success)
		//	{
		//		//client.ClientInfo.ConnState = Enums.ConnState.VERIFIED;
		//		client.Disconnect("Linked - success", ConnState.LINK_EXIT);

		//		//TODO: disconnect??
		//	}
		//	else
		//	{
		//		client.Disconnect("Linked - fail", ConnState.ERROR);
		//	}
		//}
	}
}
