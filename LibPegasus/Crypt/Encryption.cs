using LibPegasus.Packets;
using LibPegasus.Utils;
using Nito.Collections;
using Serilog;
using Sodium;
using Sodium.Exceptions;
using System.Buffers.Binary;
using System.Diagnostics;
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

		public int GetPacketSize(ReadOnlySpan<byte> encryptedData)
		{
			if (encryptedData.Length < 4)
			{
				throw new ArgumentException("Data span must contain at least 4 bytes to read packet size.", nameof(encryptedData));
			}

			// Direct zero-allocation slice of the existing span
			return (int)BinaryPrimitives.ReadUInt32LittleEndian(encryptedData.Slice(0, 4));
		}

		public byte[] Encrypt(byte[] fullArray)
		{
			if (fullArray.Length < UNENCRYPTED_SIZE)
				throw new ArgumentException("Packet is too small.", nameof(fullArray));

			var counter = BinaryPrimitives.ReadUInt64LittleEndian(
				fullArray.AsSpan(4, 8));

			if (counter == 0)
				return fullArray;

			if (_sessionKey == null)
				throw new InvalidOperationException("Session key is not initialized.");

			int payloadLength = fullArray.Length - UNENCRYPTED_SIZE;

			byte[] result = new byte[
				UNENCRYPTED_SIZE + payloadLength + ChaCha20Poly1305.TagSize];

			// Copy unencrypted header
			fullArray.AsSpan(0, UNENCRYPTED_SIZE)
				.CopyTo(result.AsSpan(0, UNENCRYPTED_SIZE));

			// Existing 8-byte counter is already your nonce
			ReadOnlySpan<byte> nonce = fullArray.AsSpan(4, 8);

			ReadOnlySpan<byte> plaintext =
				fullArray.AsSpan(UNENCRYPTED_SIZE, payloadLength);

			Span<byte> ciphertext =
				result.AsSpan(UNENCRYPTED_SIZE, payloadLength + ChaCha20Poly1305.TagSize);

			ChaCha20Poly1305.Encrypt(
				plaintext,
				nonce,
				_sessionKey,
				ciphertext);

			return result;
		}

		public ushort Decrypt(ref byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			if (data.Length < UNENCRYPTED_SIZE)
				throw new ArgumentException("Packet is too small.", nameof(data));

			var counter = BinaryPrimitives.ReadUInt64LittleEndian(
				data.AsSpan(4, 8));

			if (counter == 0)
			{
				return BinaryPrimitives.ReadUInt16LittleEndian(
					data.AsSpan(UNENCRYPTED_SIZE, 2));
			}

			if (_sessionKey == null)
				throw new InvalidOperationException("Session key is not initialized.");

			int encryptedLength = data.Length - UNENCRYPTED_SIZE;

			if (encryptedLength < ChaCha20Poly1305.TagSize)
				throw new ArgumentException("Encrypted packet is too small.", nameof(data));

			int plaintextLength = encryptedLength - ChaCha20Poly1305.TagSize;

			byte[] result = new byte[UNENCRYPTED_SIZE + plaintextLength];

			// Preserve unencrypted header.
			data.AsSpan(0, UNENCRYPTED_SIZE)
				.CopyTo(result.AsSpan(0, UNENCRYPTED_SIZE));

			// Existing counter is the 8-byte nonce.
			ReadOnlySpan<byte> nonce = data.AsSpan(4, 8);

			ReadOnlySpan<byte> ciphertext =
				data.AsSpan(UNENCRYPTED_SIZE, encryptedLength);

			Span<byte> plaintext =
				result.AsSpan(UNENCRYPTED_SIZE, plaintextLength);

			ChaCha20Poly1305.Decrypt(
				ciphertext,
				nonce,
				_sessionKey,
				plaintext);

			data = result;

			return BinaryPrimitives.ReadUInt16LittleEndian(
				result.AsSpan(UNENCRYPTED_SIZE, 2));
		}

		private bool TestEncryption()
		{
			if (_sessionKey == null)
				return false;

			UInt64 tempCounter = 1;
			var nonce = BitConverter.GetBytes(tempCounter);
			Debug.Assert(nonce.Length == 8);
			byte[] plaintext = SodiumCore.GetRandomBytes(10);
			byte[] encrypted = new byte[plaintext.Length + ChaCha20Poly1305.TagSize];
			byte[] decrypted = new byte[plaintext.Length];

			//var encrypted = SecretAeadChaCha20Poly1305.Encrypt(plaintext, nonce, _sessionKey);
			ChaCha20Poly1305.Encrypt(
				plaintext,
				nonce,
				_sessionKey,
				encrypted);

			ChaCha20Poly1305.Decrypt(
				encrypted,
				nonce,
				_sessionKey,
				decrypted);
			
			if (plaintext.SequenceEqual(decrypted))
			{
				if(plaintext.Length == 10 && encrypted.Length == 26 && decrypted.Length == 10)
				{
					Log.Debug("TestEncryption passed");
					return true;
				}
			}

			return false;
		}

		public void GenerateSessionKey(byte[] serverNonce, Byte[] clientNonce, Byte[] peerPublicKey)
		{
			byte[] sharedSecret = Sodium.ScalarMult.Mult(KeyPair.PrivateKey, peerPublicKey);
			byte[] combinedNonce = serverNonce.Concat(clientNonce).ToArray();

			_sessionKey = GenericHash.Hash(sharedSecret, combinedNonce, 32);
			//Utility.PrintByteArray(combinedNonce, combinedNonce.Length, "combinedNonce");
			//Utility.PrintByteArray(_sessionKey, _sessionKey.Length, "sessionKey");
#if DEBUG
			var pass = TestEncryption();

			if (!pass)
				throw new Exception("Encryption test failed");
#endif
		}
	}
}
