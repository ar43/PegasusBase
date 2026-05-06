using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_GetSvrTime : Packet<Client>
	{
		public REQ_GetSvrTime(Queue<byte> data) : base((UInt16)Opcode.CSC_GETSVRTIME, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => CharSelect.OnGetSvrTime(x));

			return true;
		}
	}
}
