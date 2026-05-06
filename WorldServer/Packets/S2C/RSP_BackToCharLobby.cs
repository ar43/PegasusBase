using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic;

namespace WorldServer.Packets.S2C
{
	internal class RSP_BackToCharLobby : Packet<Client>
	{
		byte _result;
		public RSP_BackToCharLobby(byte result) : base((UInt16)Opcode.CSC_BACKTOCHARLOBBY)
		{
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
		}
	}
}
