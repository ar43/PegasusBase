using System.Collections;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Items;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime
{
	internal class GroundItemManager
	{
		Dictionary<int, GroundItem> _groundItems;
		private readonly Instance _instance;
		private BitArray _takenIds;
		private int _groundItemIdGenerator = 1;
		public GroundItemManager(Instance instance)
		{
			_instance = instance;
			_takenIds = new BitArray(0xFFFF + 1);
			_groundItems = new();
		}

		private int GetNextId()
		{
			_groundItemIdGenerator++;
			if (_takenIds[_groundItemIdGenerator] == false)
			{
				return _groundItemIdGenerator;
			}
			else
			{
				throw new NotImplementedException();
			}


		}

		public void AddGroundItem(Item item, UInt32 fromId, UInt16 X, UInt16 Y, ItemContextType itemContextType)
		{
			UInt16 newKey = (UInt16)_instance.Rng.Next(0xFFFF + 1);

			GroundItem groundItem = new(GetNextId(), item, X, Y, itemContextType, newKey, fromId);
			_groundItems[groundItem.ObjectId] = groundItem;
			_instance.AddGroundItemToCell(groundItem, groundItem.CellX, groundItem.CellY, true);
		}

		public void RemoveGroundItem(GroundItem groundItem)
		{
			groundItem.Delete();
			_instance.RemoveGroundItemFromCell(groundItem, true);
			_groundItems.Remove(groundItem.ObjectId);
		}

		internal Item? OnLootRequest(Client client, int objectId, UInt16 key, UInt32 itemKind, UInt16 slot)
		{
			throw new NotImplementedException();
			/*
			var groundItem = _groundItems[objectId];
			var questLootInfo = (0, 0, 0);

			if (groundItem == null)
				throw new Exception("ground item not found");

			if (groundItem.Key != key)
				throw new Exception("incorrect key");

			if (groundItem.Item.Kind != itemKind)
				throw new Exception("item kind mismatch");

			if (groundItem.Active == false)
				throw new Exception("item already looted");

			if (groundItem.Item.IsQuestItem())
			{
				questLootInfo = client.Character.QuestManager.NeedItem(groundItem.Item);
				if (questLootInfo == (0, 0, 0))
					throw new Exception("don't need this quest item");
			}

			if (client.Character?.Location?.Instance?.Id == _instance.Id)
			{
				Item item = groundItem.Item;
				bool addSuccess = client.Character.Inventory.AddItem(slot, item);
				if (!addSuccess)
				{
					throw new Exception("inventory desync (slot not empty?)");
				}
					

				RemoveGroundItem(groundItem);

				if (questLootInfo != (0, 0, 0))
				{
					client.Character.QuestManager.OnQuestItemLoot(questLootInfo.Item1, questLootInfo.Item2, questLootInfo.Item3);
				}

				return client.Character.Inventory.PeekItem(slot);
			}
			else
			{
				throw new Exception("instance mismatch");
			}
			*/
		}
	}
}
