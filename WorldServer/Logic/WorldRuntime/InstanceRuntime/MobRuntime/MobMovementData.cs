using LibPegasus.Utils;
using System.Diagnostics;
using WorldServer.Logic.CharData;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime
{
	internal class MobMovementData
	{
		public bool IsMoving { get; private set; }
		public int StartX { get; private set; }
		public int StartY { get; private set; }
		public int EndX { get; private set; }
		public int EndY { get; private set; }
		public Int32 StartTime { get; private set; }

		public int X { get; private set; }
		public int Y { get; private set; }

		public int CellX { get; private set; }
		public int CellY { get; private set; }

		public float MoveSpeed { get; set; }

		public MobMovementData(UInt16 startX, UInt16 startY, float moveSpeed)
		{
			IsMoving = false;
			StartX = startX;
			StartY = startY;
			EndX = startX;
			EndY = startY;
			X = startX;
			Y = startY;
			CellX = X / 16;
			CellY = Y / 16;
			StartTime = 0;
			MoveSpeed = moveSpeed;
		}

		public void UpdateCellPos()
		{
			CellX = X / 16;
			CellY = Y / 16;
		}

		public void SetPosition(int x, int y)
		{
			StartX = x;
			StartY = y;
			X = x;
			Y = y;
			EndX = x;
			EndY = y;
		}
	}
}
