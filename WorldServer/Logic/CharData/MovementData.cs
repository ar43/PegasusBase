using LibPegasus.Utils;

namespace WorldServer.Logic.CharData
{
	internal class MovementData
	{
		public static readonly int MAX_SQR_MOVE_DIFF = 256;
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

		List<Waypoint> _waypoints = new List<Waypoint>();

		public float Distance { get; private set; }
		public float Base { get; private set; }
		public float Sin { get; private set; }
		public float Cos { get; private set; }
		public bool IsDeadReckoning { get; private set; }
		public int LastDeadReckoning;
		public int IllegalMovementCounter;
		public int CurrentWaypoint { get; private set; } // thinking

		public float MoveSpeed { get; private set; }

		public MovementData(UInt16 startX, UInt16 startY, float moveSpeed)
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
			IsDeadReckoning = false;
			Distance = 0;
			Base = 0;
			Sin = 0;
			Cos = 0;
			MoveSpeed = moveSpeed;
			LastDeadReckoning = 0;
			CurrentWaypoint = 0;
			IllegalMovementCounter = 0;
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

		public bool VerifyDistanceToNpc(int npcX, int npcY)
		{
			var dx = X - npcX;
			var dy = Y - npcY;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			if (adx > 7)
			{
				Serilog.Log.Warning($"{adx} | {ady}");
				return false;
			}

			if (ady > 7)
			{
				Serilog.Log.Warning($"{adx} | {ady}");
				return false;
			}

			return true;
		}

		
	}
}
