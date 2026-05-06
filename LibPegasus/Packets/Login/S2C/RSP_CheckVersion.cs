using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;

namespace LibPegasus.Packets.Login.S2C
{
	public class RSP_CheckVersion<ClientClass> : Packet<ClientClass>
	{
		private UInt32 _clientVersion;

		public RSP_CheckVersion(UInt32 clientVersion) : base((UInt16)OpcodeLogin.CHECKVERSION)
		{
			_clientVersion = clientVersion;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _clientVersion);
			PacketWriter.WriteUInt32(data, 0);
			PacketWriter.WriteUInt32(data, 0);
			PacketWriter.WriteUInt32(data, 0);

		}
	}
}
