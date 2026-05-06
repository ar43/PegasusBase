using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace LibPegasus.Packets.Login.C2S
{
	public class REQ_Connect2Svr<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass, byte[], byte[]>? OnServerConnectionHandler;

		public REQ_Connect2Svr(Queue<byte> data) : base((UInt16)OpcodeLogin.CONNECT2SVR, data)
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

		private byte[]? _clientNonce;
		private byte[]? _clientPublicKey;

		public REQ_Connect2Svr(byte[] clientNonce, byte[] clientPublicKey) : base((UInt16)OpcodeLogin.CONNECT2SVR)
		{
			_clientNonce = clientNonce;
			_clientPublicKey = clientPublicKey;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteArray(data, _clientNonce); // 8 byte
			PacketWriter.WriteArray(data, _clientPublicKey); // 32 byte
		}
	}
}
