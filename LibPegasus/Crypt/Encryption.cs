using LibPegasus.Packets;
using Nito.Collections;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace LibPegasus.Crypt
{
	public class Encryption
	{
		public static readonly UInt16 C2S_HEADER_SIZE = 14;
		public Encryption()
		{
		}

		public int GetPacketSize(Span<byte> encryptedData)
		{
			var span = new Span<byte>(encryptedData.ToArray(), 0, 4);
			UInt32 decryptedValue = BinaryPrimitives.ReadUInt32LittleEndian(span);
			return (int)decryptedValue;
		}

		public byte[] Encrypt(Deque<byte> byteQueue)
		{
			var packetLen = byteQueue.Count;
			byte[] outputBytes = byteQueue.ToArray();
			return outputBytes;
		}

		public UInt16 Decrypt(byte[] data)
		{
			var span = new Span<byte>(data, 12, 2);
			var opcode = BinaryPrimitives.ReadUInt16LittleEndian(span);

			return opcode;
		}
	}
}
