using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic;

namespace WorldServer.Packets.S2C
{
	internal class RSP_ItemDrop : Packet<Client>
	{
		byte _result;
		public RSP_ItemDrop(byte result) : base((UInt16)Opcode.CSC_ITEMDROP)
		{
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
		}
	}
}
