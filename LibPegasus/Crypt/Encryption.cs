using LibPegasus.Packets;
using LibPegasus.Utils;
using Nito.Collections;
using Serilog;
using Sodium;
using Sodium.Exceptions;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace LibPegasus.Crypt
{
	public class Encryption
	{
		public static readonly UInt16 C2S_HEADER_SIZE = 14;
		public readonly KeyPair KeyPair;
		private byte[]? _sessionKey;

		public Encryption(KeyPair keyPair)
		{
			KeyPair = keyPair;
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

		private bool TestEncryption()
		{
			if (_sessionKey == null)
				return false;

			UInt64 tempCounter = 1;
			var nonce = BitConverter.GetBytes(tempCounter);
			Debug.Assert(nonce.Length == 8);
			byte[] plaintext = RandomNumberGenerator.GetBytes(10);

			var encrypted = SecretAeadChaCha20Poly1305.Encrypt(plaintext, nonce, _sessionKey);
			var decrypted = SecretAeadChaCha20Poly1305.Decrypt(encrypted, nonce, _sessionKey);

			if(plaintext.SequenceEqual(decrypted))
			{
				Log.Debug("Sequences match");
				if(plaintext.Length == 10 && encrypted.Length == 26 && decrypted.Length == 10)
				{
					Log.Debug("Lenghts are as expected");
					return true;
				}
			}

			return false;
		}

		public void GenerateSessionKey(byte[] serverNonce, Byte[] clientNonce, Byte[] peerPublicKey)
		{
			byte[] sharedSecret = Sodium.ScalarMult.Mult(KeyPair.PrivateKey, peerPublicKey);
			byte[] combinedNonce = serverNonce.Concat(clientNonce).ToArray();
			byte[] info = Encoding.UTF8.GetBytes("pegasus");

			_sessionKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, 32, combinedNonce, info);
			Utility.PrintByteArray(combinedNonce, combinedNonce.Length, "combinedNonce");
			Utility.PrintByteArray(_sessionKey, _sessionKey.Length, "sessionKey");
			var pass = TestEncryption();

			if (!pass)
				throw new Exception("Encryption test failed");
		}
	}
}
