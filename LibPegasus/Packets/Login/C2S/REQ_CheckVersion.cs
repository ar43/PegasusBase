using LibPegasus.Enums;
using LibPegasus.Packets;
namespace LibPegasus.Packets.Login.C2S
{
	public class REQ_CheckVersion<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass, UInt32>? OnCheckVersionHandler;
		public REQ_CheckVersion(Queue<byte> data) : base((UInt16)OpcodeLogin.CHECKVERSION, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			UInt32 clientVersion;

			try
			{
				clientVersion = PacketReader.ReadUInt32(_data);
				PacketReader.ReadDiscard(_data, 4 * 3);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			if (OnCheckVersionHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnCheckVersionHandler?.Invoke(x, clientVersion));

			return true;
		}
	}
}
