using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class RSP_Connect2Svr : Packet<Client>
	{
		private UInt32 _authKey;
		private UInt16 _userIdx;
		private byte[] _serverNonce;
		private byte[] _publicServerKey;

		public RSP_Connect2Svr(UInt32 authKey, UInt16 userIdx, byte[] serverNonce, byte[] publicServerKey) : base((UInt16)Opcode.CONNECT2SVR)
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
	}
}
