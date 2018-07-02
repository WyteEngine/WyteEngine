using System.Collections;
namespace WyteEngine.Item
{
	/// <summary>
	/// 水分を多く含む，喉も潤い，お腹もいっぱいになる食べ物のアイテム．
	/// </summary>
	public class ItemFoodIncludingMoisture : ItemBase, IDrinkableItem, IEatableItem
	{
		public ItemFoodIncludingMoisture(string textureId, string itemName, int water, int food, string description) : base(textureId, itemName, description)
		{
			WaterPoint = water;
			FoodPoint = food;
		}

		public int WaterPoint { get; }
		public int FoodPoint { get; }

		public override IEnumerator OnUse(object user, ItemEventArgs e)
		{
			var isWet = CurrentPlayer.WaterPoint >= PlayerData.MaxWaterPoint;
			var isFull = CurrentPlayer.FoodPoint >= PlayerData.MaxFoodPoint;

			if (isFull && isWet)
			{
				yield return e.Say($"{e.UserName}は　{e.ItemName}を　食べようとしたが\nお腹がいっぱいだった！");
				e.Canceled = true;
				yield break;
			}

			// 飲む
			if (!isWet)
			{
				CurrentPlayer.WaterPoint += WaterPoint;

				// todo 回復音を追加する

				// 矯正
				if (CurrentPlayer.WaterPoint > PlayerData.MaxWaterPoint)
					CurrentPlayer.WaterPoint = PlayerData.MaxWaterPoint;
			}

			// 食べる
			if (!isFull)
			{
				CurrentPlayer.FoodPoint += FoodPoint;

				// todo 回復音を追加する

				// 矯正
				if (CurrentPlayer.FoodPoint > PlayerData.MaxFoodPoint)
					CurrentPlayer.FoodPoint = PlayerData.MaxFoodPoint;
			}

			yield return e.Say($"{e.UserName}は　{e.ItemName}を　食べた！！\n");
		}
	}
}