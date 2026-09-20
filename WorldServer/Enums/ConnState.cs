namespace WorldServer.Enums
{
	internal enum ConnState : UInt16
	{
		INITIAL,
		AWAITING,
		AUTHORIZING,
		CONNECTED,
		KICKED,
		TIMEOUT,
		ERROR,
		AWAITING_LINK_REPLY,
		LINK_EXIT,
		DISCONNECTED
	}
}
