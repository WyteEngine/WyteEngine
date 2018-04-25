using System.Collections;

public class ItemUsableKey : ItemUsable, IItemKey
{
	public ItemUsableKey(string textureId, string itemName, string label, string description) : base(textureId, itemName, label, description) { }
}
