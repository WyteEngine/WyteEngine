namespace WyteEngine.Item
{
	/// <summary>
	/// 食べられるアイテムのインターフェイスです．
	/// </summary>
	public interface IEatableItem : IFoodItem
	{
		/// <summary>
		/// 食用時の満腹度回復値．
		/// </summary>
		/// <value>The food point.</value>
		int FoodPoint { get; }
	}
}