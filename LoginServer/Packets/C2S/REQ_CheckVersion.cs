using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_CheckVersion<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass, UInt32>? OnCheckVersionHandler;
		public REQ_CheckVersion(Queue<byte> data) : base((UInt16)Opcode.CHECKVERSION, data)
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
