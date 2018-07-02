using System.Collections;
namespace WyteEngine.Item
{
	public class ItemKey : ItemBase, IItemKey
	{
		public ItemKey(string textureId, string itemName, string description) : base(textureId, itemName, description) { }

		public override IEnumerator OnUse(object user, ItemEventArgs args)
		{
			return args.Say("今は　使い時ではない。");
		}

	}
}