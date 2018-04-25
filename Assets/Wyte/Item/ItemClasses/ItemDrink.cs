using System.Collections;

/// <summary>
/// 飲み物のアイテム．
/// </summary>
public class ItemDrink : ItemBase, IDrinkableItem
{
	public ItemDrink(string textureId, string itemName, int water, string description) : base(textureId, itemName, description)
	{
		WaterPoint = water;
	}

	public int WaterPoint { get; }

	public override IEnumerator OnUse(object user, ItemEventArgs e)
	{
		if (CurrentPlayer.WaterPoint >= PlayerData.MaxWaterPoint)
		{
			yield return e.Say($"{e.UserName}は　{e.ItemName}を　飲もうとしたが\n特に　喉が渇いていなかった！");
			e.Canceled = true;
			yield break;
		}

		CurrentPlayer.WaterPoint += WaterPoint;

		// todo 回復音を追加する

		// 矯正
		if (CurrentPlayer.WaterPoint > PlayerData.MaxWaterPoint)
			CurrentPlayer.WaterPoint = PlayerData.MaxWaterPoint;

		yield return e.Say($"{e.UserName}は　{e.ItemName}を　飲んだ！\n");
	}
}
