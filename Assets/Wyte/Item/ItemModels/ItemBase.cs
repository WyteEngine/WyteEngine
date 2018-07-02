using System.Collections;
using UnityEngine;

namespace WyteEngine.Item
{
	public abstract class ItemBase : IItem
	{
		public string TextureId { get; }

		public string ItemName { get; }

		public string Description { get; }

		public virtual IEnumerator OnUse(object user, ItemEventArgs args)
		{
			// デフォルト
			return args.Say($"{args.UserName} は {args.ItemName} をつかった！\nしかし　特に何も　起こらなかった...");
		}

		protected ItemBase(string textureId, string itemName, string description)
		{
			TextureId = textureId;
			ItemName = itemName;
		}

		protected PlayerData CurrentPlayer => GameMaster.Instance.Player;
	}
}