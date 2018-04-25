using System.Collections;

public class ItemUsable : ItemBase
{
	public string Label { get; }
	public ItemUsable(string textureId, string itemName, string label, string description) : base(textureId, itemName, description)
	{
		Label = label;
	}

	public override IEnumerator OnUse(object user, ItemEventArgs e)
	{
		return EventController.Instance.Runtime.Call(Label);
	}
}
