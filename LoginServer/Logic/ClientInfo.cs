using LoginServer.Enums;
using System.Security.Cryptography;

namespace LoginServer.Logic
{
	internal class ClientInfo
	{
		public static readonly int RSA_KEY_SIZE = 2048;

		public ClientInfo(UInt32 userId, UInt32 authKey)
		{
			UserId = userId;
			AuthKey = authKey;
			ConnState = ConnState.INITIAL;
			Username = "";
			AccountId = 0;
			ServerNonce = RandomNumberGenerator.GetBytes(8);
		}

		public UInt32 UserId { get; private set; }
		public UInt32 AuthKey { get; private set; }
		public string Username;
		public ConnState ConnState;
		public byte[] ServerNonce { get; private set; }

		public UInt32 AccountId;
	}
}
