using LibPegasus.Utils;
using System.Diagnostics;
using System.Numerics;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime
{
	internal class Mob
	{
		private MobData _data;
		private MobSpawnData _spawnData;
		private readonly Instance _instance;
		public UInt16 ObjectId { get; private set; }
		public MobMovementData Movement { get; private set; }
		public int HP { get; private set; } //temp, will be replaced by some Status class
		public int Level { get; private set; }
		public byte Nation { get; private set; }
		public bool IsSpawned { get; private set; } = false;
		public bool IsDead { get; private set; } = false;
		public bool IsChasing { get; private set; } = false;
		public int AddedAlertRange { get; private set; }
		private List<(int, int)>? _spawnSpots = null;
		private DateTime _nextUpdateTime;
		private Random _rng;

		private const int MAX_REACTION_RANGE = 16;

		public AggroTable AggroTable { get; private set; }
		public Client? LastAttacker { get; private set; }
		public Client? CurrentDefender { get; private set; }
		public bool IsAttacked { get; private set; }
		private MobPhase _phase;

		private int spawnInterval = 5000;

		public Mob(MobData data, MobSpawnData spawnData, Instance instance, UInt16 id, Random rng)
		{
			_data = data;
			_spawnData = spawnData;
			_instance = instance;
			ObjectId = id;
			Nation = 0;
			Movement = new(0, 0, _data.MoveSpeed);
			_nextUpdateTime = DateTime.MinValue;
			_rng = rng;
			_phase = MobPhase.INVALID;
			AggroTable = new AggroTable();
			LastAttacker = null;
			CurrentDefender = null;
			IsAttacked = false;

			if (_instance.Type == Enums.InstanceType.FIELD)
				AddedAlertRange = 2;
			else
				AddedAlertRange = 32;

		}

		private void SetNextUpdateTime(DateTime currentTime, int ms)
		{
			_nextUpdateTime = currentTime;
			_nextUpdateTime = _nextUpdateTime.AddTicks(TimeSpan.FromMilliseconds(ms).Ticks);
		}

		public UInt16 GetSpecies()
		{
			if (_spawnData.SpeciesIdx != _data.Id)
				throw new Exception("Fatal mob data error");
			return (UInt16)_spawnData.SpeciesIdx;
		}

		private List<(int, int)> CalculateValidSpawnSpots(UInt16 baseX, UInt16 baseY, int width, int height)
		{
			List<(int, int)> values = new List<(int, int)>();

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (!_instance.CheckTileMoveDisable((UInt16)(baseX + i), (UInt16)(baseY + j)) && !_instance.CheckTileTown((UInt16)(baseX + i), (UInt16)(baseY + j)))
					{
						values.Add((baseX + i, baseY + j));
					}
				}
			}
			return values;
		}

		public void SetAttacker(Client? client)
		{
			if (client == null)
			{
				LastAttacker = null;
				IsAttacked = false;
			}
			else
			{
				LastAttacker = client;
				IsAttacked = true;
			}
		}

		public int GetMaxHP()
		{
			return _data.HP;
		}


		public void Spawn(DateTime currentTime)
		{
			if (_spawnSpots == null)
				_spawnSpots = CalculateValidSpawnSpots((UInt16)_spawnData.PosX, (UInt16)_spawnData.PosY, _spawnData.Width, _spawnData.Height);
			if (_spawnSpots.Count <= 0)
				throw new Exception("No valid spawn spots");

			var position = _spawnSpots[_rng.Next(_spawnSpots.Count)];
			var newX = position.Item1;
			var newY = position.Item2;

			if (IsSpawned)
				_instance.RemoveMobFromCell(this, true, Enums.DelObjectType.WARP);

			Movement = new(0, 0, _data.MoveSpeed);
			Movement.SetPosition(newX, newY);
			Movement.UpdateCellPos();
			IsSpawned = true;
			IsDead = false;
			//todo: phases
			//SetPhaseFind(currentTime, true);
			UnselectTarget();
			SetAttacker(null);

			Level = (Byte)(_data.LEV + _rng.Next(3));
			HP = GetMaxHP();

			_instance.AddMobToCell(this, (UInt16)Movement.CellX, (UInt16)Movement.CellY, true);
		}

		private bool IsTooFarFromSpawn()
		{
			throw new NotImplementedException();
		}

		private static int GetDistance(int x1, int y1, int x2, int y2)
		{
			int iDx = x2 - x1, iDy = y2 - y1;


			if (iDx < 0) iDx = -iDx;
			if (iDy < 0) iDy = -iDy;

			return (iDx >= iDy) ? iDx : iDy;
		}

		private void UnselectTarget()
		{
			CurrentDefender = null;
			AggroTable.Reset();
		}

		private void SwitchAttackerToDefender()
		{
			CurrentDefender = LastAttacker;
			LastAttacker = null;
		}


		private void Find(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.FIND);
		}

		private void AssertStill()
		{
			Debug.Assert(Movement.StartX == Movement.EndX);
			Debug.Assert(Movement.StartY == Movement.EndY);
			Debug.Assert(Movement.X == Movement.StartX);
			Debug.Assert(Movement.Y == Movement.StartY);
		}

		private void Battle(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.BATTLE);
		}

		//public DamageFromMobResult ResolveAttack(Client defender, MobSkill skill)
		//{
			
		//}

		//public int CalculateNormalDamageTaken(Character attacker, Skill skill, int attack)
		//{
			
		//}

		public void Update(DateTime currentTime)
		{
			if (currentTime < _nextUpdateTime)
				return;

			if (!IsSpawned && IsDead)
			{
				Serilog.Log.Debug($"Spawning mob {ObjectId}");
				Spawn(currentTime);
				return;
			}
			else if (!IsSpawned)
			{
				throw new Exception("Not supposed to happen");
			}

			switch (_phase)
			{
				case MobPhase.FIND:
				{
					Find(currentTime);
					break;
				}
				case MobPhase.BATTLE:
				{
					Battle(currentTime);
					break;
				}
				//....
				default:
				{
					throw new Exception("Invalid phase");
				}
			}
		}

		internal void TakeDamage(Int32 damage)
		{
			HP -= damage;
		}

		private void DropItem(Client attacker)
		{

		}

		private void DropQuestItem(Client attacker)
		{
		}

		internal void Kill(DelObjectType delObjectType = DelObjectType.DEAD, Client? attacker = null, int skillId = 0)
		{
			HP = 0;
			DeathCheck(attacker, skillId, true, delObjectType);
		}

		internal void Delete(DelObjectType delObjectType = DelObjectType.DEAD, Client? attacker = null, int skillId = 0)
		{
			HP = 0;
			_nextUpdateTime = DateTime.MaxValue;
			DeathCheck(attacker, skillId, true, delObjectType);
		}

		internal void DeathCheck(Client? attacker, int skillId, bool notifyAround = false, DelObjectType delObjectType = DelObjectType.WARP)
		{
			if (HP <= 0)
			{
				IsSpawned = false;
				IsDead = true;
				_instance.RemoveMobFromCell(this, notifyAround, delObjectType);
				SetNextUpdateTime(DateTime.UtcNow, spawnInterval);

				if(attacker != null)
				{
					//attacker.Character.QuestManager.OnMobDeath(attacker, (UInt16)_data.Id, skillId);
					DropQuestItem(attacker);

					DropItem(attacker);
					
				}
				
			}
		}
	}
}
