using UnityEngine;

[System.Serializable]
public struct Keys
{
	public string Left;
	public string Right;
	public string Up;
	public string Down;
	public string Jump;
	public string Action;
	public string Dash;
	public string Pause;
	public string Menu;
	public string ExLeft;
	public string ExRight;
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

public class KeyBinding : SingletonBaseBehaviour<KeyBinding> {

	public static readonly Keys Default = new Keys("left", "right", "up", "down", "z", "x", "left shift", "escape", "a", "q", "w", false);

	public Keys Binding
	{
		get { return _binding; }
		set { _binding = value; }
	}

	private Keys _binding;

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
