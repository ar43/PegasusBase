using LibPegasus.Enums;
using LibPegasus.Packets;

namespace LibPegasus.Packets.Login.C2S
{
	public class REQ_PreServerEnvRequest<ClientClass> : Packet<ClientClass>
	{
		public static Action<ClientClass, string>? OnPreServerEnvRequestHandler;
		public REQ_PreServerEnvRequest(Queue<byte> data) : base((UInt16)OpcodeLogin.PRESERVERENVREQUEST, data)
		{

		}

		public override bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			string username;

			try
			{
				username = PacketReader.ReadString(_data, 33);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			if (OnPreServerEnvRequestHandler == null)
				throw new InvalidOperationException("Handler not assigned");

			actions.Enqueue((x) => OnPreServerEnvRequestHandler?.Invoke(x, username));

			return true;
		}
	}
}
