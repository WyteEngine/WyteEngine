using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TextManager : SingletonBaseBehaviour<TextManager>
{
	readonly Dictionary<string, TextObject> map = new Dictionary<string, TextObject>();

	[SerializeField]
	RectTransform host;

	/// <summary>
	/// ホストが存在しなければ例外をスローします．
	/// </summary>
	void Assert(string id = null)
	{
		if (host == null)
			throw new System.InvalidOperationException($"{nameof(TextManager)} にテキストコンポーネントのホストが指定されていません．");

		if (id != null && !map.ContainsKey(id))
		throw new System.ArgumentNullException(nameof(id));

	}

	public void Create(string id, string text, string locationMode = "middle_center", Vector2 point = default(Vector2), TextAnchor textAlignment = TextAnchor.MiddleCenter, Color color = default(Color))
	{
		Assert();
		// 既にあれば上書きする
		if (map.ContainsKey(id))
			Delete(id);
		
		map[id] = new TextObject(new GameObject(id, typeof(Text), typeof(RectTransform)));
		
		// テキストのサイズを自動調整するコンポーネント
		var fitter = map[id].Object.AddComponent<ContentSizeFitter>();
		fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		map[id].Rect.SetParent(host);

		Move(id, point, locationMode);
		Modify(id, text, textAlignment, color);

	}

	public void Move(string id, Vector2 point, string mode = null)
	{
		Assert(id);
		var r = map[id].Rect;

		// 素直にアンカーに従う
		r.anchoredPosition = point;

		if (mode != null)
		{
			// モード変更処理
			// ピボットもアンカーと同じにすることで自然になる
			r.anchorMin = r.anchorMax = r.pivot = GetAnchor(mode);
		}
	}

	public void Modify(string id, string text, TextAnchor? textAlignment = null, Color? color = null)
	{
		Assert(id);
		
		var obj = map[id].Text;

		obj.text = text;

		obj.alignment = textAlignment.HasValue ? textAlignment.Value : obj.alignment;
		obj.color = color.HasValue ? color.Value : obj.color;
	}

	public void Delete(string id)
	{
		Assert(id);

		// いなければ無視
		if (!map.ContainsKey(id))
			return;
		
		Destroy(map[id].Object);
		map.Remove(id);
	}

	struct TextObject
	{
		public RectTransform Rect { get; private set; }
		public GameObject Object => Rect.gameObject;
		public Text Text { get; private set; }

		public TextObject(GameObject go)
		{
			Rect = go.transform as RectTransform;
			Text = go.GetComponent<Text>();
		}
	}

	#region novel api
	public IEnumerator TxtSet(string id, params string[] args)
	{
		id = GetTag(id, ref args);
		NArgsAssert(args.Length > 0);
		var mode = default(string);
		var point = default(Vector2);
		var align = TextAnchor.MiddleCenter;
		var color = Color.black;

		// テキスト
		var text = args[0];

		if (args.Length > 1)
		{
			// 座標モード
			mode = args[1];
		}
		if (args.Length > 3)
		{
			// 座標
			float x, y;
			NArgsAssert(float.TryParse(args[2], out x));
			NArgsAssert(float.TryParse(args[3], out y));
			point = new Vector2(x, y);
		}
		if (args.Length > 4)
		{
			// テキスト位置
			align = TextAlignmentStringToEnum(args[4]);
		}
		if (args.Length > 5)
		{
			// 色
			NArgsAssert(ColorUtility.TryParseHtmlString(args[5], out color));
		}

		Create(id, text, mode, point, align, color);

		yield break;
	}

	public IEnumerator TxtOfs(string id, params string[] args)
	{
		id = GetTag(id, ref args);
		Assert(id);
		NArgsAssert(args.Length >= 2);
		string mode = null;
		float lerpTime = 0;
	
		// 座標
		float x, y;
		NArgsAssert(float.TryParse(args[0], out x));
		NArgsAssert(float.TryParse(args[1], out y));
		var point = new Vector2(x, y);
		if (args.Length >= 3)
		{
			// 座標モード
			mode = args[2];
		}
		if (args.Length >= 4)
		{
			// 線形補間時間
			NArgsAssert(float.TryParse(args[3], out lerpTime));
		}

		// 非同期で動かす
		StartCoroutine(TxtMove(lerpTime, id, point, mode));

		yield break;
	}

	public IEnumerator TxtMod(string id, params string[] args)
	{
		id = GetTag(id, ref args);
		
		NArgsAssert(args.Length >= 1);
		var mode = default(TextAnchor?);
		var color = default(Color?);
		// テキスト
		var text = args[0];
		
		// 座標モード
		if (args.Length >= 2)
		{
			mode = TextAlignmentStringToEnum(args[1]);
		}

		if (args.Length >= 3)
		{
			Color col;
			NArgsAssert(ColorUtility.TryParseHtmlString("", out col));
			color = col;
		}

		Modify(id, text, mode, color);
		yield break;
	}

	public IEnumerator TxtClr(string id, params string[] args)
	{
		Delete(GetTag(id, ref args));
		yield break;
	}

	private IEnumerator TxtMove(float lerpTime, string id, Vector2 target, string mode)
	{
		if (lerpTime > 0)
		{
			var time = 0f;
			var beforePoint = map[id].Rect.anchoredPosition;
			while (time <= lerpTime)
			{
				map[id].Rect.anchoredPosition = Vector2.Lerp(beforePoint, target, time / lerpTime);
				time += Time.deltaTime;
				yield return null;
			}
		}

		Move(id, target, mode);
	}

	#endregion

	public string GetTag(string tag, ref string[] args)
	{
		NArgsAssert(args.Length > 0);
		if (string.IsNullOrEmpty(tag))
		{
			tag = args[0];
			args = args.Take(1).ToArray();
		}
		return tag;
	}

	#region static helper

	public static TextAnchor TextAlignmentStringToEnum(string mode)
	{
		switch (mode.ToLower())
		{
			case "left":
				return TextAnchor.MiddleLeft;
			case "center":
				return TextAnchor.MiddleCenter;
			case "right":
				return TextAnchor.MiddleRight;
			default:
				throw new System.ArgumentException("不正な文字列です．");
		}
	}

	public static Vector2 GetAnchor(string mode)
	{
		var modes = mode.ToLower().Split('_');
		if (modes.Length != 2)
		{
			throw new System.ArgumentException("座標指定モードの文字列が正しくありません．");
		}

		var vec = default(Vector2);

		vec.x = modes[1] == "left" ? 0 : modes[1] == "center" ? .5f : modes[1] == "right" ? 1 : 0;
		vec.y = modes[0] == "top" ? 1 : modes[0] == "middle" ? .5f : modes[0] == "bottom" ? 0 : 0;

		return vec;
	}
#endregion
}

/// <summary>
/// テキストの座標指定モード．
/// </summary>
public enum LocationMode
{
	   TopLeft,    TopCenter,    TopRight,
	MiddleLeft, MiddleCenter, MiddleRight,
	BottomLeft, BottomCenter, BottomRight,
}
