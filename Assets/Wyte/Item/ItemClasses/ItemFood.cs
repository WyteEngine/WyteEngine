using System.Collections;

public class ItemFood : ItemBase, IEatableItem
{
	public ItemFood(string textureId, string itemName, int food, string description) : base(textureId, itemName, description)
	{
		FoodPoint = food;
	}

	public int FoodPoint { get; }

	public override IEnumerator OnUse(object user, ItemEventArgs e)
	{
		if (CurrentPlayer.WaterPoint >= PlayerData.MaxWaterPoint)
		{
			yield return e.Say($"{e.UserName}は　{e.ItemName}を　食べようとしたが\nお腹がいっぱいだった！");
			e.Canceled = true;
			yield break;
		}

		CurrentPlayer.FoodPoint += FoodPoint;

		// todo 回復音を追加する

		// 矯正
		if (CurrentPlayer.FoodPoint > PlayerData.MaxFoodPoint)
			CurrentPlayer.FoodPoint = PlayerData.MaxFoodPoint;

		yield return e.Say($"{e.UserName}は　{e.ItemName}を　食べた！！\n");
	}
}