using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace LibPegasus.Crypt
{
	public static unsafe class ChaCha20Poly1305
	{
		public const int KeySize = 32;
		public const int NonceSize = 8;
		public const int TagSize = 16;

		[DllImport("libsodium", CallingConvention = CallingConvention.Cdecl)]
		private static extern int sodium_init();

		[DllImport(
			"libsodium",
			CallingConvention = CallingConvention.Cdecl,
			EntryPoint = "crypto_aead_chacha20poly1305_encrypt")]
		private static extern int EncryptNative(
			byte* c,
			ulong* clen,
			byte* m,
			ulong mlen,
			byte* ad,
			ulong adlen,
			byte* nsec,
			byte* npub,
			byte* k);

		static ChaCha20Poly1305()
		{
			if (sodium_init() < 0)
				throw new CryptographicException("libsodium initialization failed.");
		}

		[DllImport(
			"libsodium",
			CallingConvention = CallingConvention.Cdecl,
			EntryPoint = "crypto_aead_chacha20poly1305_decrypt")]
		private static extern int DecryptNative(
			byte* m,
			ulong* mlen,
			byte* nsec,
			byte* c,
			ulong clen,
			byte* ad,
			ulong adlen,
			byte* npub,
			byte* k);

		public static int Decrypt(
			ReadOnlySpan<byte> ciphertext,
			ReadOnlySpan<byte> nonce,
			ReadOnlySpan<byte> key,
			Span<byte> plaintext)
		{
			if (key.Length != KeySize)
				throw new ArgumentException(
					$"Key must be exactly {KeySize} bytes.",
					nameof(key));

			if (nonce.Length != NonceSize)
				throw new ArgumentException(
					$"Nonce must be exactly {NonceSize} bytes.",
					nameof(nonce));

			if (ciphertext.Length < TagSize)
				throw new ArgumentException(
					"Ciphertext is too small.",
					nameof(ciphertext));

			int requiredLength = ciphertext.Length - TagSize;

			if (plaintext.Length < requiredLength)
				throw new ArgumentException(
					$"Plaintext buffer must be at least {requiredLength} bytes.",
					nameof(plaintext));

			ulong plaintextLength = 0;

			fixed (byte* m = plaintext)
			fixed (byte* c = ciphertext)
			fixed (byte* npub = nonce)
			fixed (byte* k = key)
			{
				int result = DecryptNative(
					m,
					&plaintextLength,
					null,
					c,
					(ulong)ciphertext.Length,
					null,
					0,
					npub,
					k);

				if (result != 0)
					throw new CryptographicException(
						"libsodium decryption/authentication failed.");
			}

			return checked((int)plaintextLength);
		}

		public static int Encrypt(
			ReadOnlySpan<byte> plaintext,
			ReadOnlySpan<byte> nonce,
			ReadOnlySpan<byte> key,
			Span<byte> ciphertext)
		{
			if (key.Length != KeySize)
				throw new ArgumentException(
					$"Key must be {KeySize} bytes.",
					nameof(key));

			if (nonce.Length != NonceSize)
				throw new ArgumentException(
					$"Nonce must be {NonceSize} bytes.",
					nameof(nonce));

			int requiredSize = plaintext.Length + TagSize;

			if (ciphertext.Length < requiredSize)
				throw new ArgumentException(
					$"Ciphertext buffer must be at least {requiredSize} bytes.",
					nameof(ciphertext));

			fixed (byte* cPtr = ciphertext)
			fixed (byte* mPtr = plaintext)
			fixed (byte* noncePtr = nonce)
			fixed (byte* keyPtr = key)
			{
				ulong ciphertextLength = 0;

				int result = EncryptNative(
					cPtr,
					&ciphertextLength,
					mPtr,
					(ulong)plaintext.Length,
					null, // no additional data
					0,
					null, // nsec unused
					noncePtr,
					keyPtr);

				if (result != 0)
					throw new CryptographicException(
						$"libsodium encryption failed with error code {result}.");

				return checked((int)ciphertextLength);
			}
		}
	}
}
