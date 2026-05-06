using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class RSP_PublicKey : Packet<Client>
	{
		private byte[] _key;

		public RSP_PublicKey(byte[] key) : base((UInt16)Opcode.PUBLICKEY)
		{
			_key = key;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, 1);
			PacketWriter.WriteUInt16(data, (ushort)_key.Length);
			PacketWriter.WriteArray(data, _key);
		}
	}
}
