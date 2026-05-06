using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;

namespace LibPegasus.Packets.Login.S2C
{
	public class RSP_PreServerEnvRequest<ClientClass> : Packet<ClientClass>
	{
		public RSP_PreServerEnvRequest() : base((UInt16)OpcodeLogin.PRESERVERENVREQUEST)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteNull(data, 4113);
		}
	}
}
