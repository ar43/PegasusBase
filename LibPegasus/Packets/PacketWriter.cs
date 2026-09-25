using Google.Protobuf;
using Nito.Collections;
using System.Text;

namespace LibPegasus.Packets
{
	public static class PacketWriter
	{
		public static void WriteByte(Deque<byte> data, byte val)
		{
			data.AddToBack(val);
		}

		public static void WriteHeaderUInt16(Deque<byte> data, UInt16 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			data.AddToFront((byte)b);
			data.AddToFront((byte)a);
		}

		public static void WriteHeaderUInt32(Deque<byte> data, UInt32 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			var c = (val & 0xFF0000) >> 16;
			var d = (val & 0xFF000000) >> 24;
			data.AddToFront((byte)d);
			data.AddToFront((byte)c);
			data.AddToFront((byte)b);
			data.AddToFront((byte)a);
		}

		public static void WriteHeaderUInt64(Deque<byte> data, UInt64 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			var c = (val & 0xFF0000) >> 16;
			var d = (val & 0xFF000000) >> 24;
			var e = (val & 0xFF00000000) >> 32;
			var f = (val & 0xFF0000000000) >> 40;
			var g = (val & 0xFF000000000000) >> 48;
			var h = (val & 0xFF00000000000000) >> 56;
			data.AddToFront((byte)h);
			data.AddToFront((byte)g);
			data.AddToFront((byte)f);
			data.AddToFront((byte)e);
			data.AddToFront((byte)d);
			data.AddToFront((byte)c);
			data.AddToFront((byte)b);
			data.AddToFront((byte)a);
		}

		public static void WriteArray(Deque<byte> data, ReadOnlySpan<byte> input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				data.AddToBack(input[i]);
			}
		}
	}
}
