/// <summary>
/// 飲めるアイテムのインターフェイスです．
/// </summary>
public interface IDrinkableItem : IFoodItem
{
	/// <summary>
	/// 飲用時の水分回復値．
	/// </summary>
	int WaterPoint { get; }
}