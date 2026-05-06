using LibPegasus.Crypt;
using Nito.Collections;
using Serilog;

namespace LibPegasus.Packets
{
	public class Packet<ClientClass>
	{
		private readonly UInt16 _id;
		protected readonly int _packetLen;
		protected Queue<byte> _data;

		public static readonly UInt16 HEADER_SIZE = 14;

		public Packet(UInt16 id, Queue<byte> data)
		{
			_id = id;
			_data = data;
			_packetLen = data.Count;
		}

		public Packet(UInt16 id)
		{
			_id = id;
			_data = new();
			_packetLen = 0;
		}

		public bool ReadHeader(UInt64 recvCounter)
		{
			if(_packetLen == 0)
				throw new NotImplementedException("_packetLen == 0. Was this meant to be read?");

			try
			{
				var len = PacketReader.ReadUInt32(_data);
				var counter = PacketReader.ReadUInt64(_data);
				var opcode = PacketReader.ReadUInt16(_data);

				//TODO: counter validation

				if (len != _packetLen)
				{
					Log.Error("packet len does not match");
					return false;
				}

				if (counter != recvCounter)
				{
					Log.Error($"counter not in sync: expected {recvCounter} - got {counter}");
					return false;
				}

				if (opcode != (UInt16)_id)
				{
					Log.Error($"opcode does not match: got {opcode} expected {_id}");
					return false;
				}
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			return true;
		}

		public bool Verify()
		{
			return _data.Count == 0;
		}

		public virtual bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			throw new NotImplementedException();
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
