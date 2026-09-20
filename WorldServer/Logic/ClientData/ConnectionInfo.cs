using System.Security.Cryptography;
using WorldServer.Enums;

namespace WorldServer.Logic.ClientData
{
	internal class ConnectionInfo
	{
		public ConnectionInfo(UInt16 userId, UInt32 authKey)
		{
			UserId = userId;
			AuthKey = authKey;
			ConnState = ConnState.INITIAL;
			AccountId = 0;
			ServerNonce = RandomNumberGenerator.GetBytes(8);
		}

		public UInt16 UserId { get; private set; }
		public UInt32 AuthKey { get; private set; }

		public bool IsAuthenticated()
		{
			return AccountId > 0 && ConnState == ConnState.CONNECTED;
		}

		public ConnState ConnState;

		public bool RequestedBackToCharLobby = false;

		public UInt32 AccountId { get; private set; }
		private bool _accIdLock = false;

		public bool SubPasswordAuthenticated = false;

		public byte[] ServerNonce { get; private set; }

		public void SetAccountId(UInt32 accountId)
		{
			if (!_accIdLock)
				AccountId = accountId;
			_accIdLock = true;
		}

	}
}
