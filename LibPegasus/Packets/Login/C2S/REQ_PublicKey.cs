using LibPegasus.Enums;
using LibPegasus.Packets;

namespace LibPegasus.Packets.Login.C2S
{
	public class REQ_PublicKey<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass>? OnPublicKeyRequestHandler;
		public REQ_PublicKey(Queue<byte> data) : base((UInt16)OpcodeLogin.PUBLICKEY, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			if (OnPublicKeyRequestHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnPublicKeyRequestHandler?.Invoke(x));

			return true;
		}
	}
}
