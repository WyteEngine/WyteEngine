using System.Collections;

/// <summary>
/// 全てのアイテムクラスはこれを実装します．
/// </summary>
public interface IItem
{
	/// <summary>
	/// このアイテムが使用する，ItemManagerに登録されているテクスチャのID．
	/// </summary>
	string TextureId { get; }

	/// <summary>
	/// アイテム名．
	/// </summary>
	string ItemName { get; }

	/// <summary>
	/// このアイテムが使用されるときに呼ばれます．
	/// </summary>
	/// <returns>コルーチン．</returns>
	/// <param name="user">アイテムを使用したオブジェクト．</param>
	/// <param name="args">このイベント用の引数です．</param>
	IEnumerator OnUse(object user, ItemEventArgs e);
}
