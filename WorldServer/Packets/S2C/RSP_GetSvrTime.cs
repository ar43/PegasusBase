using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic;

namespace WorldServer.Packets.S2C
{
	internal class RSP_GetSvrTime : Packet<Client>
	{
		long _time;
		Int16 _timezoneOffset;
		public RSP_GetSvrTime(long time, Int16 timezoneOffset) : base((UInt16)Opcode.CSC_GETSVRTIME)
		{
			_time = time;
			_timezoneOffset = timezoneOffset;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt64(data, (ulong)_time);
			PacketWriter.WriteInt16(data, _timezoneOffset);
		}
	}
}
