using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;

namespace LibPegasus.Packets.Login.S2C
{
	public class RSP_PublicKey<ClientClass> : Packet<ClientClass>
	{
		private byte[] _key;

		public RSP_PublicKey(byte[] key) : base((UInt16)OpcodeLogin.PUBLICKEY)
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
