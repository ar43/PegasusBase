using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_Connect2Svr : PacketC2S<Client>
	{
		public REQ_Connect2Svr(Queue<byte> data) : base((UInt16)Opcode.CONNECT2SVR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte[] clientNonce;
			byte[] clientPublicKey;
			try
			{
				clientNonce = PacketReader.ReadArray(_data, 8);
				clientPublicKey = PacketReader.ReadArray(_data, 32);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnServerConnection(x, clientNonce, clientPublicKey));

			return true;
		}
	}
}
