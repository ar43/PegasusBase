using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic;

namespace WorldServer.Packets.S2C
{
	internal class NFY_SMastUpEvnt : Packet<Client>
	{
		byte _newMasteryLvl;

		public NFY_SMastUpEvnt(Byte newMasteryLvl) : base((UInt16)Opcode.NFY_SMASTUPEVNT)
		{
			_newMasteryLvl = newMasteryLvl;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _newMasteryLvl);
		}
	}
}
