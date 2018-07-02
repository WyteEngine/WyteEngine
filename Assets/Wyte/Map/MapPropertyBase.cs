using UnityEngine;

namespace WyteEngine.Map
{
	/// <summary>
	/// Wyte Engineで取り扱うすべてのマップコンポーネントを表します．
	/// </summary>
	public abstract class MapPropertyBase : MonoBehaviour
	{
		/// <summary>
		/// このマップを初期化します．
		/// </summary>
		/// <param name="m">M.</param>
		public abstract void Initialize(MapManager m);

		/// <summary>
		/// サイズを取得または設定します．
		/// </summary>
		public Rect Size { get; protected set; }

		/// <summary>
		/// 背景色．
		/// </summary>
		public Color BackColor;

		/// <summary>
		/// 背景画像．
		/// </summary>
		public Wallpaper Background;

		/// <summary>
		/// 前景画像．
		/// </summary>
		public Wallpaper Foreground;
	}
}