using UnityEngine;
using WyteEngine.Helper;
using WyteEngine.UI;

namespace WyteEngine.Inputing
{
	public class KeyBinding : SingletonBaseBehaviour<KeyBinding>
	{

		public static readonly Keys Default = new Keys("left", "right", "up", "down", "z", "x", "left shift", "escape", "a", "q", "w", false);

		public Keys Binding { get; set; }

		private static readonly string BindingFileName = "keybind.json";

		public void Save()
		{
			SaveDataHelper.Save(BindingFileName, Binding);
		}

		public Keys Load()
		{
			var data = SaveDataHelper.Load<Keys>(BindingFileName);
			return Binding = data.Equals(default(Keys)) ? Default : data;
		}

		private void Start()
		{
			Load();
			if (Binding.Equals(Default)) Save();
		}

	}

	[System.Serializable]
	public struct Keys
	{
		/// <summary>
		/// 左キー。
		/// </summary>
		public string Left;
		/// <summary>
		/// 右キー。
		/// </summary>
		public string Right;
		/// <summary>
		/// 上キー。
		/// </summary>
		public string Up;
		/// <summary>
		/// 下キー。
		/// </summary>
		public string Down;
		/// <summary>
		/// ジャンプキー。
		/// </summary>
		public string Jump;
		/// <summary>
		/// アクションキー。
		/// </summary>
		public string Action;
		/// <summary>
		/// ダッシュキー。
		/// </summary>
		public string Dash;
		/// <summary>
		/// ポーズキー。
		/// </summary>
		public string Pause;
		/// <summary>
		/// メニューキー。
		/// </summary>
		public string Menu;
		/// <summary>
		/// 拡張左キー。
		/// </summary>
		public string ExLeft;
		/// <summary>
		/// 拡張右キー。
		/// </summary>
		public string ExRight;
		/// <summary>
		/// タッチを強制するかどうか。
		/// </summary>
		public bool ForceTouch;

		/// <summary>
		/// 方向キーに対応したベクトル値を取得します。x軸は右、y軸は上方向としています。
		/// </summary>
		public Vector2Int Arrow =>
			EnvironmentFlag.IsSmartDevice
				? new Vector2Int(
					GamePadBehaviour.Instance.Get(GamePadButtons.Left) ? -1 : GamePadBehaviour.Instance.Get(GamePadButtons.Right) ? 1 : 0, 0)
				: new Vector2Int(
								   Input.GetKey(Left) ? -1 : Input.GetKey(Right) ? 1 : 0,
								   Input.GetKey(Up) ? 1 : Input.GetKey(Down) ? -1 : 0
				);


		public Keys(string left, string right, string up, string down, string jump, string action, string dash, string pause, string menu, string exLeft, string exRight, bool touch)
		{
			Left = left;
			Right = right;
			Up = up;
			Down = down;
			Jump = jump;
			Action = action;
			Dash = dash;
			Pause = pause;
			Menu = menu;
			ExLeft = exLeft;
			ExRight = exRight;
			ForceTouch = touch;
		}

	}

}