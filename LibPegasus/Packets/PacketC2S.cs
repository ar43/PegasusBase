using LibPegasus.Crypt;
using Serilog;

namespace LibPegasus.Packets
{
	public class PacketC2S<ClientClass>
	{
		private readonly UInt16 _id;
		protected readonly int _packetLen;
		protected Queue<byte> _data;

		public PacketC2S(UInt16 id, Queue<byte> data)
		{
			_id = id;
			_data = data;
			_packetLen = data.Count;
		}

		public bool ReadHeader(UInt64 recvCounter)
		{
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
	}
}
