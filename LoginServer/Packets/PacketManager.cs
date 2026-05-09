using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;
using LibPegasus.Packets.Login.C2S;
using LibPegasus.Enums;
using Nito.Collections;
using Serilog;

namespace LoginServer.Packets
{
	internal class PacketManager
	{
		private Queue<Tuple<UInt16, Queue<byte>>> _decryptedInboundPackets = new();
		private Queue<Deque<byte>> _decryptedOutboundPackets = new();
		public DanglingPacket? DanglingPacket = null;

		private UInt64 _sendCounter = 0;
		private UInt64 _recvCounter = 0;

		public PacketManager()
		{
			REQ_Connect2Svr<Client>.OnServerConnectionHandler = Connection.OnServerConnection;
			REQ_AuthAccount<Client>.OnAuthAccountHandler = Connection.OnAuthAccount;
			REQ_CheckVersion<Client>.OnCheckVersionHandler = Connection.OnCheckVersion;
			REQ_VerifyLinks<Client>.OnVerifyLinksHandler = Connection.OnVerifyLinks;
		}

		public void EnqueuePacket(UInt16 opcode, Queue<byte> packet)
		{
			_decryptedInboundPackets.Enqueue(new Tuple<UInt16, Queue<byte>>(opcode, packet));
		}

		public Deque<byte> GetOutboundPacket()
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

		private Packet<Client> GetPacket(OpcodeLogin opcode, Queue<byte> data)
		{
			return opcode switch
			{
				OpcodeLogin.CONNECT2SVR => new REQ_Connect2Svr<Client>(data),
				OpcodeLogin.CHECKVERSION => new REQ_CheckVersion<Client>(data),
				OpcodeLogin.AUTHACCOUNT => new REQ_AuthAccount<Client>(data),
				OpcodeLogin.VERIFYLINKS => new REQ_VerifyLinks<Client>(data),
				_ => throw new NotImplementedException($"unimplemented opcode {opcode}"),
			}; ;
		}

		public Queue<Action<Client>>? ReceiveAll()
		{
			Queue<Action<Client>>? actions = null;

			while (_decryptedInboundPackets.Count > 0)
			{
				if (actions == null)
					actions = new Queue<Action<Client>>();

				var packetInfo = _decryptedInboundPackets.Dequeue();
				var opcodeNum = packetInfo.Item1;
				var dataQueue = packetInfo.Item2;

				bool opcodeDefined = Enum.IsDefined(typeof(OpcodeLogin), opcodeNum);
				if (!opcodeDefined)
				{
					Log.Warning($"Received undefined opcode {opcodeNum}(len={dataQueue.Count})");
					_recvCounter++;
					continue;
				}

				var packet = GetPacket((OpcodeLogin)opcodeNum, dataQueue);
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
