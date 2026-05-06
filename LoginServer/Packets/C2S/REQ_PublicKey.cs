using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_PublicKey<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass>? OnPublicKeyRequestHandler;
		public REQ_PublicKey(Queue<byte> data) : base((UInt16)Opcode.PUBLICKEY, data)
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
