using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;

namespace LibPegasus.Packets.Login.S2C
{
	public class NFY_UrlToClient<ClientClass> : Packet<ClientClass>
	{
		public NFY_UrlToClient() : base((UInt16)OpcodeLogin.URLTOCLIENT)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
			// no idea what this is, just copying whatever actual EP8 server sends
			PacketWriter.WriteUInt16(data, 22);
			PacketWriter.WriteUInt16(data, 20);
			PacketWriter.WriteNull(data, 5 * 4);
		}
	}
}
