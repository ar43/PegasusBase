using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;
using System.Text;

namespace LibPegasus.Packets.Login.S2C
{
	public class NFY_SystemMessg<ClientClass> : Packet<ClientClass>
	{
		LoginMessageType _msgType;
		string _msg;
		public NFY_SystemMessg(LoginMessageType msgType, String msg) : base((UInt16)OpcodeLogin.SYSTEMMESSG)
		{
			_msgType = msgType;
			_msg = msg;

			_msg ??= "";
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, (byte)_msgType);
			PacketWriter.WriteUInt16(data, (UInt16)_msg.Length);
			if (_msg.Length > 0)
			{
				PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(_msg));
				PacketWriter.WriteByte(data, 0); //possibly need a null byte here or perhaps not
			}
		}
	}
}
