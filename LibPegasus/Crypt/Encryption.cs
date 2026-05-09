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
		public static readonly UInt16 UNENCRYPTED_SIZE = 12;
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
			byte[] fullArray = byteQueue.ToArray();
			var counterSpan = new Span<byte>(fullArray, 4, 8);
			var headerSpan = new Span<byte>(fullArray, 0, UNENCRYPTED_SIZE);
			var counter = BinaryPrimitives.ReadUInt64LittleEndian(counterSpan);

			if (counter == 0)
			{
				return fullArray;
			}
			else
			{
				if (_sessionKey == null)
					throw new NullReferenceException();

				

				var toEncryptSpan = new Span<byte>(fullArray, UNENCRYPTED_SIZE, fullArray.Length - UNENCRYPTED_SIZE);
				var encrypted = SecretAeadChaCha20Poly1305.Encrypt(toEncryptSpan.ToArray(), BitConverter.GetBytes(counter), _sessionKey);
				byte[] result = new byte[encrypted.Length + UNENCRYPTED_SIZE];
				headerSpan.CopyTo(result);
				encrypted.CopyTo(result, headerSpan.Length);
				return result;
			}
		}

		public UInt16 Decrypt(ref byte[] data)
		{
			var packetLen = data.Length;
			byte[] fullArray = data;
			var counterSpan = new Span<byte>(fullArray, 4, 8);
			var headerSpan = new Span<byte>(fullArray, 0, UNENCRYPTED_SIZE);
			var counter = BinaryPrimitives.ReadUInt64LittleEndian(counterSpan);

			if (counter == 0)
			{
				var span = new Span<byte>(data, UNENCRYPTED_SIZE, 2);
				var opcode = BinaryPrimitives.ReadUInt16LittleEndian(span);
				return opcode;
			}
			else
			{
				if (_sessionKey == null)
					throw new NullReferenceException();

				var encrypted = new Span<byte>(data, UNENCRYPTED_SIZE, data.Length - UNENCRYPTED_SIZE);
				var decrypted = SecretAeadChaCha20Poly1305.Decrypt(encrypted.ToArray(), BitConverter.GetBytes(counter), _sessionKey);
				byte[] result = new byte[decrypted.Length + UNENCRYPTED_SIZE];
				headerSpan.CopyTo(result);
				decrypted.CopyTo(result, headerSpan.Length);
				data = result;

				var span = new Span<byte>(data, UNENCRYPTED_SIZE, 2);
				var opcode = BinaryPrimitives.ReadUInt16LittleEndian(span);
				return opcode;
			}
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
			//Utility.PrintByteArray(combinedNonce, combinedNonce.Length, "combinedNonce");
			//Utility.PrintByteArray(_sessionKey, _sessionKey.Length, "sessionKey");
			var pass = TestEncryption();

			if (!pass)
				throw new Exception("Encryption test failed");
		}
	}
}
