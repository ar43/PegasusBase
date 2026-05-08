using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;

namespace LibPegasus.Packets.Login.S2C
{
	public class RSP_CheckVersion<ClientClass> : Packet<ClientClass>
	{
		private UInt32 _serverVersion;
		public static Action<ClientClass, UInt32>? OnCheckVersionHandler;
		public RSP_CheckVersion(Queue<byte> data) : base((UInt16)OpcodeLogin.CHECKVERSION, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			try
			{
				_serverVersion = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			if (OnCheckVersionHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnCheckVersionHandler?.Invoke(x, _serverVersion));

			return true;
		}

		public RSP_CheckVersion(UInt32 serverVersion) : base((UInt16)OpcodeLogin.CHECKVERSION)
		{
			_serverVersion = serverVersion;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _serverVersion);

		}
	}
}
