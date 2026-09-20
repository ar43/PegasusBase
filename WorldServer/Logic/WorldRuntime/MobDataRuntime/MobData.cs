using WorldServer.Enums;

namespace WorldServer.Logic.WorldRuntime.MobDataRuntime
{
	internal class MobData
	{
		public MobData(Int32 id, Single moveSpeed, Int32 lEV, Int32 hP)
		{
			Id = id;
			MoveSpeed = moveSpeed;
			LEV = lEV;
			HP = hP;
		}

		public int Id { get; private set; }
		public float MoveSpeed { get; private set; }
		public int LEV { get; private set; }
		public int HP { get; private set; }
	}
}
