using LibPegasus.Enums;
using LibPegasus.Packets;

namespace LibPegasus.Packets.Login.C2S
{
	public class REQ_VerifyLinks<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass, UInt32, UInt16, byte, byte, UInt32>? OnVerifyLinksHandler;
		public REQ_VerifyLinks(Queue<byte> data) : base((UInt16)OpcodeLogin.VERIFYLINKS, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			UInt32 authKey;
			UInt16 userId;
			byte channelId;
			byte serverId;
			UInt32 clientMagicKey;

			try
			{
				authKey = PacketReader.ReadUInt32(_data);
				userId = PacketReader.ReadUInt16(_data);
				channelId = PacketReader.ReadByte(_data);
				serverId = PacketReader.ReadByte(_data);
				clientMagicKey = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			if (OnVerifyLinksHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnVerifyLinksHandler?.Invoke(x, authKey, userId, channelId, serverId, clientMagicKey));

			return true;
		}
	}
}
