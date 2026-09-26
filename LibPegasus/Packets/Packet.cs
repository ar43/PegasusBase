using LibPegasus.Crypt;
using Nito.Collections;
using Serilog;

namespace LibPegasus.Packets
{
	public class Packet<ClientClass>
	{
		private readonly UInt16 _id;
		protected readonly int _packetLen;
		protected byte[]? _data;

		public static readonly UInt16 HEADER_SIZE = 14;

		public Packet(UInt16 id, byte[] data)
		{
			_id = id;
			_data = data;
			_packetLen = data.Length;
		}

		public Packet(UInt16 id)
		{
			_id = id;
			_data = null;
			_packetLen = 0;
		}

		public bool ReadHeader(UInt64 recvCounter)
		{
			if(_packetLen == 0)
				throw new NotImplementedException("_packetLen == 0. Was this meant to be read?");
			if (_data == null)
				throw new NullReferenceException("null _data");

			try
			{
				var len = PacketReader.ReadUInt32(_data, 0);
				var counter = PacketReader.ReadUInt64(_data, 4);
				var opcode = PacketReader.ReadUInt16(_data, 12);

				//TODO: counter validation

				if(counter == 0)
				{
					if (len != _packetLen)
					{
						Log.Error("packet len does not match");
						return false;
					}
				}
				else
				{
					if (len-16 != _packetLen)
					{
						Log.Error("packet len does not match");
						return false;
					}
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

		public virtual bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			throw new NotImplementedException();
		}

		private void WriteHeader(byte[] data, UInt64 counter)
		{
			var size = (UInt32)(data.Length);
			if (counter == 0)
				PacketWriter.WriteUInt32(data, 0, size); //for first unencrypted packet
			else
				PacketWriter.WriteUInt32(data, 0, size + (uint)16); //for encrypted packets

			PacketWriter.WriteUInt64(data, 4, counter);
			PacketWriter.WriteUInt16(data, 12, _id);
		}

		public byte[] Send(UInt64 counter)
		{
			byte[] data = WritePayload();
			WriteHeader(data, counter);
			return data;
		}

		public virtual byte[] WritePayload()
		{
			throw new NotImplementedException();
		}
	}
}
