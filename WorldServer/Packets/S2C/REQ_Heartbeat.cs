using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic;

namespace WorldServer.Packets.S2C
{
	internal class REQ_Heartbeat : Packet<Client>
	{
		public REQ_Heartbeat() : base((UInt16)Opcode.CSC_HEARTBEAT)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
		}
	}
}
