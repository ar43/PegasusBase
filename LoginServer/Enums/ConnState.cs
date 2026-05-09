namespace LoginServer.Enums
{
	internal enum ConnState
	{
		INITIAL,
		CONNECTED,
		VERSION_CHECKED,
		AUTH_ACCOUNT,
		VERIFYING,
		VERIFIED
	}
}
