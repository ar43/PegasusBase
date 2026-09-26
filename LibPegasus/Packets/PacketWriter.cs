using Google.Protobuf;
using Nito.Collections;
using System.Text;

namespace LibPegasus.Packets
{
	public static class PacketWriter
	{
		public static void WriteUInt32(byte[] data, int offset, uint val)
		{
			data[offset] = (byte)(val & 0xFF);
			data[offset + 1] = (byte)((val >> 8) & 0xFF);
			data[offset + 2] = (byte)((val >> 16) & 0xFF);
			data[offset + 3] = (byte)((val >> 24) & 0xFF);
		}

		public static void WriteUInt16(byte[] data, int offset, UInt16 val)
		{
			data[offset] = (byte)(val & 0xFF);
			data[offset + 1] = (byte)((val >> 8) & 0xFF);
		}

		public static void WriteUInt64(byte[] data, int offset, ulong val)
		{
			data[offset] = (byte)(val & 0xFF);
			data[offset + 1] = (byte)((val >> 8) & 0xFF);
			data[offset + 2] = (byte)((val >> 16) & 0xFF);
			data[offset + 3] = (byte)((val >> 24) & 0xFF);
			data[offset + 4] = (byte)((val >> 32) & 0xFF);
			data[offset + 5] = (byte)((val >> 40) & 0xFF);
			data[offset + 6] = (byte)((val >> 48) & 0xFF);
			data[offset + 7] = (byte)((val >> 56) & 0xFF);
		}
	}
}
