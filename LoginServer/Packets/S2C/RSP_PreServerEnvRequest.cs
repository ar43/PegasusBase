using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class RSP_PreServerEnvRequest : Packet<Client>
	{
		public RSP_PreServerEnvRequest() : base((UInt16)Opcode.PRESERVERENVREQUEST)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteNull(data, 4113);
		}
	}
}
