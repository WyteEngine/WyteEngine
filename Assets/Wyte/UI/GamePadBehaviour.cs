using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

[DefaultExecutionOrder(-1000)]
public class GamePadBehaviour : SingletonBaseBehaviour<GamePadBehaviour> {

	PlayerController player;

	public RectTransform Left, Right, Action, Menu, SliderUp, SliderDown, SliderLeft, SliderRight, Escape;
	private Text actionText;
	public Text Haribote;

	// Use this for initialization
	void Start ()
	{
		if (!IsSmartDevice)
			gameObject.SetActive(false);
		Input.multiTouchEnabled = true;
		actionText = Action.gameObject.GetComponentInChildren<Text>();
	}

	// Update is called once per frame
	void Update() {
		if (actionText != null && Wyte.CurrentPlayer != null)
		{
			actionText.text = Wyte.CurrentPlayer.CurrentNpc == null ? "↑" : "…";
		}
		Haribote.text = DateTime.Now.ToString("T");

	}

	public bool Get(GamePadButtons gpb, bool down = false)
	{
		foreach (var t in Input.touches)
		{
			if (down && t.phase != TouchPhase.Began)
				continue;
			switch (gpb)
			{
				case GamePadButtons.Left:
					if (!Left) return false;
					if (Left.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.Right:
					if (!Right) return false;
					if (Right.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.Action:
					if (!Action) return false;
					if (Action.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.Menu:
					if (!Menu) return false;
					if (Menu.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.SliderUp:
					if (!SliderUp) return false;
					if (SliderUp.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.SliderDown:
					if (!SliderDown) return false;
					if (SliderDown.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.SliderLeft:
					if (!SliderLeft) return false;
					if (SliderLeft.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.SliderRight:
					if (!SliderRight) return false;
					if (SliderRight.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.Escape:
					if (!Escape) return false;
					if (Escape.Overlaps(t.position)) return true;
					break;
				case GamePadButtons.Screen:
					// パッドに触れていればtrue => falseならスクリーンに触れている => notをとる
					if (!GetComponent<RectTransform>().Overlaps(t.position)) return true;
					break;
				default:
					throw new System.ArgumentException("予期しないボタンの判定を試みました。");
			}
		}
		return false;
	}

	/*private void OnGUI()
	{
		GUILayout.Label($@"{Input.touchCount}
------
{string.Join("\n", Input.touches.Select(t => t.position.ToString()))}");
	}*/

}

public enum GamePadButtons
{
	Left,
	Right,
	Action,
	Menu,
	SliderUp,
	SliderDown,
	SliderLeft,
	SliderRight,
	Escape,
	Screen
}

internal static class Extension
{
	public static bool Overlaps(this RectTransform r, Vector2 point)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(r, point, Camera.main);
		//return (rect.xMin < point.x) && (point.x < rect.xMax) && (rect.yMin < point.y) && (point.y < rect.yMax);
	}
}
