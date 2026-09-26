using System.Buffers.Binary;

namespace LibPegasus.Packets
{
	public static class PacketReader
	{

		public static UInt16 ReadUInt16(byte[] data, int offset)
		{
			return BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset, 2));
		}

		public static UInt32 ReadUInt32(byte[] data, int offset)
		{
			return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
		}

		public static UInt64 ReadUInt64(byte[] data, int offset)
		{
			return BinaryPrimitives.ReadUInt64LittleEndian(data.AsSpan(offset, 8));
		}

	}
}
