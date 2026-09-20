using LibPegasus.Crypt;
using LibPegasus.Enums;
using LibPegasus.Packets.World;
using LibPegasus.Packets.World.S2C;
using WorldServer.Enums;

namespace WorldServer.Logic.Delegates
{
	internal static class Connection
	{
		internal static void OnServerConnection(Client client, UInt32 clientVersion, byte[] clientNonce, byte[] clientPublicKey)
		{
			var cfg = ServerConfig.Get();
			if (client.ConnectionInfo.ConnState != ConnState.INITIAL)
			{
				client.Disconnect("invalid handshake", ConnState.ERROR);
				return;
			}

			if (clientVersion != WorldPacketVersion.Revision)
			{
				client.Disconnect($"version mismatch, client: {clientVersion} server: {WorldPacketVersion.Revision}", ConnState.ERROR);
				return;
			}

			client.ConnectionInfo.ConnState = Enums.ConnState.AWAITING;

			client.Encryption.GenerateSessionKey(client.ConnectionInfo.ServerNonce, clientNonce, clientPublicKey);

			var packet = new RSP_Connect2Svr<Client>(client.ConnectionInfo.AuthKey, client.ConnectionInfo.UserId,
				client.ConnectionInfo.ServerNonce, client.Encryption.KeyPair.PublicKey);
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
