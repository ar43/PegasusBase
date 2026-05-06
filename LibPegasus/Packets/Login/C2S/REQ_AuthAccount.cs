using LibPegasus.Enums;
using LibPegasus.Packets;

namespace LibPegasus.Packets.Login.C2S
{
	public class REQ_AuthAccount<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass, byte[]>? OnAuthAccountHandler;
		public REQ_AuthAccount(Queue<byte> data) : base((UInt16)OpcodeLogin.AUTHACCOUNT, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			byte[] rsaData;

			try
			{
				PacketReader.ReadDiscard(_data, 2);
				rsaData = PacketReader.ReadArray(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			if (OnAuthAccountHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnAuthAccountHandler?.Invoke(x, rsaData));

			return true;
		}
	}
}
