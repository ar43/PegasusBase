using LibPegasus.Enums;
using LibPegasus.Packets;
using LibPegasus.Packets.World.C2S;
using Nito.Collections;
using Serilog;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets
{
	internal class PacketManager
	{
		private Queue<Tuple<UInt16, Queue<byte>>> _decryptedInboundPackets = new();
		private Queue<byte[]> _decryptedOutboundPackets = new();
		public DanglingPacket? DanglingPacket = null;

		private UInt64 _sendCounter = 0;
		private UInt64 _recvCounter = 0;

		public PacketManager()
		{
			REQ_ConnectServer<Client>.ConnectServerHandler = Connection.ConnectServerHandler;
		}

		public void EnqueuePacket(UInt16 opcode, Queue<byte> packet)
		{
			_decryptedInboundPackets.Enqueue(new Tuple<UInt16, Queue<byte>>(opcode, packet));
		}

		public byte[] GetOutboundPacket()
		{
			return _decryptedOutboundPackets.Dequeue();
		}

		public bool OutputQueued()
		{
			return _decryptedOutboundPackets.Count > 0;
		}

		public void Send(Packet<Client> packet)
		{
			var p = packet.Send(_sendCounter);
			_decryptedOutboundPackets.Enqueue(p);
			_sendCounter++;
		}

		private Packet<Client> GetPacket(OpcodeWorld opcode, Queue<byte> data)
		{
			return opcode switch
			{
				OpcodeWorld.CONNECTSERVER => new REQ_ConnectServer<Client>(data),
				_ => throw new NotImplementedException($"unimplemented opcode {opcode}"),
			}; ;
		}

		public Queue<Action<Client>>? ReceiveAll(bool isAuthenticated)
		{
			Queue<Action<Client>>? actions = null;

			while (_decryptedInboundPackets.Count > 0)
			{
				if (actions == null)
					actions = new Queue<Action<Client>>();

				var packetInfo = _decryptedInboundPackets.Dequeue();
				var opcodeNum = packetInfo.Item1;
				var dataQueue = packetInfo.Item2;

				bool opcodeDefined = Enum.IsDefined(typeof(OpcodeWorld), opcodeNum);
				if (!opcodeDefined)
				{
					Log.Warning($"Received undefined opcode {opcodeNum}(len={dataQueue.Count})");
					_recvCounter++;
					continue;
				}

				if (!isAuthenticated && (OpcodeWorld)opcodeNum != OpcodeWorld.CONNECTSERVER)
				{
					Log.Warning($"Received opcode {opcodeNum}(len={dataQueue.Count}) while unauthenticated");
					_recvCounter++;
					continue;
				}

				var packet = GetPacket((OpcodeWorld)opcodeNum, dataQueue);
				Log.Debug($"Processing opcode {opcodeNum}");

				bool verifyHeader = packet.ReadHeader(_recvCounter);
				_recvCounter++;
				if (!verifyHeader)
				{
					Log.Warning($"Header for opcode {opcodeNum} is invalid");
					continue;
				}

				bool ok = packet.ReadPayload(actions);
				if (!ok)
				{
					Log.Warning($"Invalid payload data during opcode {opcodeNum}");
					continue;
				}

				bool verifyReceived = packet.Verify();
				if (!verifyReceived)
				{
					Log.Warning($"Data of opcode {opcodeNum} was not fully read");
					continue;
				}
			}
			return actions;
		}
	}
}
