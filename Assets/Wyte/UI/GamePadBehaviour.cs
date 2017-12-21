using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class GamePadBehaviour : SingletonBaseBehaviour<GamePadBehaviour> {

	PlayerController player;

	public RectTransform Left, Right, Action, Menu, SliderUp, SliderDown, SliderLeft, SliderRight, Escape;

	// Use this for initialization
	void Start ()
	{
		if (!IsSmartDevice)
			gameObject.SetActive(false);
		Input.multiTouchEnabled = true;
	}

	// Update is called once per frame
	void Update() {
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
					return false;
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
	Escape
}

internal static class Extension
{
	public static bool Overlaps(this RectTransform r, Vector2 point)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(r, point, Camera.main);
		//return (rect.xMin < point.x) && (point.x < rect.xMax) && (rect.yMin < point.y) && (point.y < rect.yMax);
	}

	
}
