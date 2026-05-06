using LibPegasus.Enums;
using LibPegasus.Packets;
using Nito.Collections;
using System.Net;

namespace LibPegasus.Packets.Login.S2C
{
	public class NFY_ServerState<ClientClass> : Packet<ClientClass>
	{
		public NFY_ServerState() : base((UInt16)OpcodeLogin.SERVERSTATE)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
			/*
			PacketWriter.WriteByte(data, (byte)_reply.ServerCount);
			for (int i = 0; i < _reply.ServerCount; i++)
			{
				var server = _reply.Servers[i];
				PacketWriter.WriteByte(data, (byte)server.ServerId);
				PacketWriter.WriteByte(data, (byte)server.ServerFlag);
				PacketWriter.WriteUInt32(data, 0); // LanguageMaybe
				PacketWriter.WriteByte(data, (byte)server.ChannelCount);
				for (int j = 0; j < server.ChannelCount; j++)
				{
					var chan = server.Channels[j];
					var ip = BitConverter.ToUInt32(IPAddress.Parse(chan.Ip).GetAddressBytes(), 0);
					PacketWriter.WriteByte(data, (byte)chan.ChannelId);
					PacketWriter.WriteUInt16(data, (UInt16)chan.UserCount);
					PacketWriter.WriteNull(data, 21); //check ostara packet
					PacketWriter.WriteByte(data, 0xFF); // maximum rank
					PacketWriter.WriteUInt16(data, (UInt16)chan.MaximumUserCount);
					PacketWriter.WriteUInt32(data, ip);
					PacketWriter.WriteUInt16(data, (UInt16)chan.Port);
					PacketWriter.WriteUInt32(data, chan.Type);
				}
			}
			*/
			throw new NotImplementedException();
		}
	}
}
