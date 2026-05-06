using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;
using System.Net.Sockets;

namespace LoginServer.Packets.C2S
{
	internal class REQ_Connect2Svr<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass, byte[], byte[]>? OnServerConnectionHandler;

		public REQ_Connect2Svr(Queue<byte> data) : base((UInt16)Opcode.CONNECT2SVR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions) 
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

			if (OnServerConnectionHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnServerConnectionHandler?.Invoke(x, clientNonce, clientPublicKey));

			return true;
		}
	}
}
