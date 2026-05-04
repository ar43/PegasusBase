using LibPegasus.Crypt;
using Nito.Collections;

namespace LibPegasus.Packets
{
	public class PacketS2C
	{
		private readonly UInt16 _id;

		public static readonly UInt16 HEADER_SIZE = 14;

		public PacketS2C(UInt16 id)
		{
			this._id = id;
		}

		private void WriteHeader(Deque<byte> data, UInt64 counter)
		{
			// in reverse order
			var size = (UInt16)(HEADER_SIZE + data.Count);
			PacketWriter.WriteHeaderUInt16(data, (UInt16)_id);
			PacketWriter.WriteHeaderUInt64(data, (UInt64)counter);
			PacketWriter.WriteHeaderUInt32(data, size);
		}

		public Deque<byte> Send(UInt64 counter)
		{
			Deque<byte> data = new();
			WritePayload(data);
			WriteHeader(data, counter);
			return data;
		}

		public virtual void WritePayload(Deque<byte> data)
		{
			throw new NotImplementedException();
		}
	}
}
