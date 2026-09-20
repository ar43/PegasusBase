using WorldServer.Enums;
using WorldServer.Logic.CharData.Quests;
using WorldServer.Logic.CharData.Skills;

namespace WorldServer.Logic.CharData
{
	internal class Character
	{
		public Character(String name, Location loc)
		{
			Name = name;
			Location = loc;
		}
		public string Name { get; set; }
		public int Id { get; private set; }
		public UInt16 ObjectId { get; private set; }
		public Location Location { get; private set; }
		public DBSyncPriority SyncPending { get; private set; }
		public bool UninitOnSync { get; private set; }


		public void Sync(DBSyncPriority prio, bool uninitOnSync = false)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
			if (uninitOnSync)
				UninitOnSync = uninitOnSync;
		}

		public void ClearSync()
		{
			SyncPending = DBSyncPriority.NONE;
			Location.Sync(DBSyncPriority.NONE);
		}

		private void OnDeath()
		{
			throw new NotImplementedException();
		}
	}
}
