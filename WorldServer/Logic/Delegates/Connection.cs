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

		internal static async void LinkBackHandler(Client client, LinkBackReq req)
		{
			var reply = await client.SendLoginSessionRequest(req.AuthKey, (UInt16)req.UserId, 0, 0);
			var packet = new RSP_LinkBack<Client>(new LinkBackRsp
			{
				SessionResult = reply.Result,
			});
			client.PacketManager.Send(packet);

			client.Disconnect("Posted link to loginm server", ConnState.LINK_EXIT);
		}
	}
}
