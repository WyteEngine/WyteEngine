using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;
using WyteEngine.Item;

namespace WyteEngine.Entities.Inventory
{
	public class PlayerInventory
	{

		public const int CommonSlotSize = 4 * 8;
		public const int FoodSlotSize = 8;
		public const int BodySlotSize = 4;

		public ItemStack[] CommonSlot { get; protected set; }
		public ItemStack[] FoodSlot { get; protected set; }
		public ItemStack[] BodySlot { get; protected set; }

		public List<ItemStack> KeyItemSlot { get; protected set; }

		readonly PlayerData player;

		public PlayerInventory(PlayerData pd)
		{
			player = pd;

			CommonSlot = new ItemStack[CommonSlotSize];
			FoodSlot = new ItemStack[FoodSlotSize];
			BodySlot = new ItemStack[BodySlotSize];
			KeyItemSlot = new List<ItemStack>();
		}

		public void Swap(ItemStack[] slot1, int item1, ItemStack[] slot2, int item2)
		{
			if (item1 >= slot1.Length || item1 < 0)
				throw new ArgumentOutOfRangeException(nameof(item1));
			if (item2 >= slot2.Length || item2 < 0)
				throw new ArgumentOutOfRangeException(nameof(item2));

			var item = slot1[item1];
			slot1[item1] = slot2[item2];
			slot2[item2] = item;
		}

		public IEnumerator<ItemStack> GetEnumerator()
			=> CommonSlot
				.Concat(FoodSlot)
				.Concat(BodySlot)
				.Concat(KeyItemSlot)
				.GetEnumerator();

		public void Swap(ItemStack[] slot, int item1, int item2) => Swap(slot, item1, slot, item2);

		public void Give(string id, int amount = 1)
		{
			var item = ItemManager.Instance[id];
			var stack = new ItemStack(id, amount);

			if (item == null)
				throw new ArgumentException($"Invalid Item ID '{id}'");

			//重複してるところに入れてみる
			foreach (var i in this)
			{
				if (i.Id == id)
				{
					i.Add(amount);
					return;
				}
			}
			//なければ空いているところに入れてみる
			if (item is IItemKey)
			{
				KeyItemSlot.Add(stack);
				return;
			}

			if (item is IFoodItem)
			{
				var index = FoodSlot.Select((x, i) => new { x, i })
					.FirstOrDefault(o => o.x == default(ItemStack));
				if (index != null)
				{
					FoodSlot[index.i] = stack;
				}
			}

			// otherwise
			{
				var index = CommonSlot.Select((x, i) => new { x, i })
						.FirstOrDefault(o => o.x == default(ItemStack));
				if (index != null)
				{
					CommonSlot[index.i] = stack;
				}
			}

			//それでもダメならエラー投げる
			throw new InventoryIsFullException();
		}

		public void Take(IList<ItemStack> slot, string id, int amount = 1)
		{
			for (int i = 0; i < slot.Count; i++)
			{
				if (slot[i].Id == id)
				{
					slot[i].Add(-amount);
					if (slot[i].Amount < 1)
						slot[i] = default(ItemStack);
					return;
				}
			}

		}

		public IEnumerator UseItem(ItemStack[] slot, int index)
		{
			if (slot.Length >= index)
				throw new ArgumentOutOfRangeException(nameof(index));

			var item = slot[index].Item;

			GameMaster.Instance.IsNotFreezed = false;
			var e = new ItemEventArgs(player.Name, item.ItemName, item is IItemKey);
			yield return item.OnUse(player, e);

			if (!e.Canceled)
			{
				// アイテムの削除
				Take(slot, slot[index].Id, 1);
			}
			GameMaster.Instance.IsNotFreezed = true;
		}
	}

	public class InventoryIsFullException : Exception
	{
		public InventoryIsFullException() { }

		public InventoryIsFullException(string message) : base(message) { }

		public InventoryIsFullException(string message, Exception innerException) : base(message, innerException) { }

		protected InventoryIsFullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}