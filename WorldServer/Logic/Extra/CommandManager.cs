using WorldServer.Logic.CharData.Skills;

namespace WorldServer.Logic.Extra
{
	internal static class CommandManager
	{
		public static Dictionary<string, Action<Client, List<string>?>> CommandList = new();

		public static void Init()
		{
			CommandList.Clear();

			CommandList["kickme"] = CommandDelegates.KickMe;
			CommandList["testmsg"] = CommandDelegates.TestMsg;
			CommandList["sync"] = CommandDelegates.Sync;
		}
	}

	internal static class CommandDelegates
	{
		public static void KickMe(Client client, List<string>? args)
		{
			client.Disconnect("used kickme cmd", Enums.ConnState.KICKED);
		}

		public static void TestMsg(Client client, List<string>? args)
		{
			Serilog.Log.Debug("hello world");
			//client.SendServerMessage("hello world");
		}
		public static void Sync(Client client, List<string>? args)
		{
			//client.SendServerMessage("Synced char");

			client.Character.Sync(Enums.DBSyncPriority.NORMAL);
		}
	}
}
