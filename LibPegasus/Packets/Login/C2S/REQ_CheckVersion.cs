using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;
namespace LibPegasus.Packets.Login.C2S
{
	public class REQ_CheckVersion<ClientClass> : Packet<ClientClass>
	{
		UInt32 _clientVersion;
		public static Action<ClientClass, UInt32>? OnCheckVersionHandler;
		public REQ_CheckVersion(Queue<byte> data) : base((UInt16)OpcodeLogin.CHECKVERSION, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			try
			{
				_clientVersion = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			if (OnCheckVersionHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnCheckVersionHandler?.Invoke(x, _clientVersion));

			return true;
		}

		public REQ_CheckVersion(UInt32 clientVersion) : base((UInt16)OpcodeLogin.CHECKVERSION)
		{
			_clientVersion = clientVersion;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _clientVersion);
		}
	}
}
