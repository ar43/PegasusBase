using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;
using System.Reflection.Emit;

namespace LibPegasus.Packets.Login.S2C
{
	public class RSP_Connect2Svr<ClientClass> : Packet<ClientClass>
	{
		private UInt32 _authKey;
		private UInt16 _userIdx;
		private byte[]? _serverNonce;
		private byte[]? _publicServerKey;

		public RSP_Connect2Svr(UInt32 authKey, UInt16 userIdx, byte[] serverNonce, byte[] publicServerKey) : base((UInt16)OpcodeLogin.CONNECT2SVR)
		{
			_authKey = authKey;
			_userIdx = userIdx;
			_serverNonce = serverNonce;
			_publicServerKey = publicServerKey;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _authKey);
			PacketWriter.WriteUInt16(data, _userIdx);
			PacketWriter.WriteArray(data, _serverNonce); // 8 byte
			PacketWriter.WriteArray(data, _publicServerKey); // 32 byte
			//Serilog.Log.Debug($"Expecting to see {_recvXorKeyIdx}");
		}

		public static Action<ClientClass, UInt32, UInt16, byte[], byte[]>? OnServerConnectionHandler;

		public RSP_Connect2Svr(Queue<byte> data) : base((UInt16)OpcodeLogin.CONNECT2SVR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			UInt32 authKey;
			UInt16 userIdx;
			byte[] serverNonce;
			byte[] publicServerKey;

			try
			{
				authKey = PacketReader.ReadUInt32(_data);
				userIdx = PacketReader.ReadUInt16(_data);
				serverNonce = PacketReader.ReadArray(_data, 8);
				publicServerKey = PacketReader.ReadArray(_data, 32);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			if (OnServerConnectionHandler == null)
				return false;

			actions.Enqueue((x) => OnServerConnectionHandler?.Invoke(x, authKey, userIdx, serverNonce, publicServerKey));

			return true;
		}
	}
}
