using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;

namespace LibPegasus.Packets.Login.S2C
{
	public class RSP_VerifyLinks<ClientClass> : Packet<ClientClass>
	{
		byte _channelId, _serverId;
		bool _isVerified;
		public RSP_VerifyLinks(byte channelId, byte serverId, bool isVerified) : base((UInt16)OpcodeLogin.VERIFYLINKS)
		{
			_channelId = channelId;
			_serverId = serverId;
			_isVerified = isVerified;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _channelId);
			PacketWriter.WriteByte(data, _serverId);
			PacketWriter.WriteByte(data, Convert.ToByte(_isVerified));
		}
	}
}
