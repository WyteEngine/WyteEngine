using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
namespace WyteEngine.UI.TextFormatting
{

	public class TextManager : SingletonBaseBehaviour<TextManager>
	{
		readonly Dictionary<string, TextObject> map = new Dictionary<string, TextObject>();

		[SerializeField]
		RectTransform host;

		[SerializeField]
		Font[] fonts;

		private void Start()
		{
			Novel.Runtime
				 .Register("txtset", TxtSet)
				 .Register("txtofs", TxtOfs)
				 .Register("txtmod", TxtMod)
				 .Register("txtclr", TxtClr);
		}

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

		public void Create(string id, string text, string locationMode = "middle_center", Vector2 point = default(Vector2), TextAnchor textAlignment = TextAnchor.MiddleCenter, int fontId = 0, int size = 13, Color color = default(Color))
		{
			Assert();
			// 既にあれば上書きする
			if (map.ContainsKey(id))
				Delete(id);

			map[id] = new TextObject(new GameObject(id, typeof(Text)));
			// テキストのサイズを自動調整するコンポーネント
			var fitter = map[id].Object.AddComponent<ContentSizeFitter>();
			fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			map[id].Rect.SetParent(host, false);

			Move(id, point, locationMode);
			Modify(id, text, textAlignment, fontId, size, color);

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

		public void Modify(string id, string text, TextAnchor? textAlignment = null, int? fontId = null, int? size = null, Color? color = null)
		{
			Assert(id);

			var obj = map[id].Text;

			obj.text = TextComponent.Parse(text);

			obj.alignment = textAlignment.HasValue ? textAlignment.Value : obj.alignment;
			obj.color = color.HasValue ? color.Value : obj.color;
			obj.font = fontId.HasValue ? fonts[fontId.Value] : obj.font;
			obj.fontSize = size.HasValue ? size.Value : obj.fontSize;
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
			var fontId = 0;
			var size = 0;
			var color = Color.white;

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
				// フォントID
				NArgsAssert(int.TryParse(args[5], out fontId) && fontId < fonts.Length);
			}
			if (args.Length > 6)
			{
				// サイズ
				NArgsAssert(int.TryParse(args[6], out size));
			}
			if (args.Length > 7)
			{
				// 色
				NArgsAssert(ColorUtility.TryParseHtmlString(args[7], out color));
			}

			Create(id, text, mode, point, align, fontId, size, color);

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
			var fontId = default(int?);
			var size = default(int?);
			// テキスト
			var text = args[0];

			// 座標モード
			if (args.Length > 1)
			{
				mode = TextAlignmentStringToEnum(args[1]);
			}

			if (args.Length > 2)
			{
				int i;
				NArgsAssert(int.TryParse(args[2], out i));
				fontId = i;
			}

			if (args.Length > 3)
			{
				int i;
				NArgsAssert(int.TryParse(args[3], out i));
				size = i;
			}

			if (args.Length > 4)
			{
				Color col;
				NArgsAssert(ColorUtility.TryParseHtmlString(args[4], out col));
				color = col;
			}

			Modify(id, text, mode, fontId, size, color);
			yield break;
		}

		public IEnumerator TxtClr(string id, params string[] args)
		{
			var texts = new List<string>(args);
			if (!string.IsNullOrEmpty(id))
				texts.Insert(0, id);
			texts.ForEach(Delete);
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
				args = args.Skip(1).ToArray();
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
		TopLeft, TopCenter, TopRight,
		MiddleLeft, MiddleCenter, MiddleRight,
		BottomLeft, BottomCenter, BottomRight,
	}
}