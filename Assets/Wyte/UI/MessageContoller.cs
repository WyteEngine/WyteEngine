using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using WyteEngine.Inputing;
using WyteEngine.Event;
using WyteEngine.UI.TextFormatting.Helper;
using WyteEngine.UI.TextFormatting;

namespace WyteEngine.UI
{

	[RequireComponent(typeof(Text))]
	public class MessageContoller : SingletonBaseBehaviour<MessageContoller>
	{

		Text text;

		string buffer;

		[SerializeField]
		float cursorSpeed = 8;

		[SerializeField]
		GameObject UISmartDevice;

		[SerializeField]
		GameObject UIPersonalComputer;

		[SerializeField]
		float speed = 1 / 16f;
		string[] cursorTemp = @"＼
｜
／
―".Replace("\r\n", "\n").Split('\n');
		string Cursor => cursorTemp[(int)(Time.time * cursorSpeed) % cursorTemp.Length];

		bool quickEnabled;

		protected override void Awake()
		{
			base.Awake();

			text = GetComponent<Text>();
			buffer = "";
		}

		public static bool IsTouched => EnvironmentFlag.IsSmartDevice
						? (Input.touchCount > 0)
						: (Input.GetKey(KeyBinding.Instance.Binding.Jump));

		GameObject CurrentPad => IsSmartDevice ? UISmartDevice : UIPersonalComputer;

		private void Update()
		{
			if (Wyte.IsDebugMode)
			{
				// デバッグテキスト
				UIPersonalComputer.GetComponentInChildren<Text>().text = string.Format(Wyte.DebugModeHelp, quickEnabled ? "○" : "×");
				if (Input.GetKeyDown(KeyCode.F1))
				{
					quickEnabled = !quickEnabled;
					Debug.Log($"<color=yellow>テキスト早送りを{(quickEnabled ? "有効化" : "無効化")}しました．</color>", this);
				}
			}

			text.text = buffer;
			if (buffer.Length > 0 && text != null)
				text.text += Cursor;
		}

		public void HideBox() => CurrentPad.SetActive(false);

		public void ShowBox() => CurrentPad.SetActive(true);

		bool prevTouch;

		public IEnumerator Say(string sprite, params string[] args)
		{
			var messageSource = NovelHelper.CombineAll(args);
			// 話者がいる場合は表示
			// hack 今後もっとUIをよくする
			buffer = string.IsNullOrEmpty(sprite) ? "" : sprite + " : ";
			messageSource = TextUtility.RemoveTags(messageSource);
			TextElement[] mes;
			try
			{
				mes = new TextComponent(messageSource).Elements;
			}
			catch (FormatException ex)
			{
				mes = new TextComponent(@"$c=red;書式エラー: " + TextUtility.ToSafeString(ex.Message)).Elements;
			}
			foreach (var c in mes)
			{

				if (c.WaitTime > 0)
				{
					yield return new WaitForSeconds(c.WaitTime);
				}

				if (c.Nod)
				{
					yield return Nod();
				}

				buffer += c.ToString();
				if (!quickEnabled && c.Speed != 0)
				{
					Sfx.Play("entity.npc.saying");
					// タッチ時は早くする
					yield return new WaitForSeconds(speed / Mathf.Abs(c.Speed) / (IsTouched ? 2 : 1));
				}
			}

			yield return Nod();

			buffer = "";

		}

		public static IEnumerator Nod()
		{
			var prevTouch = true;
			// 前回タッチされてなく、かつタッチされていれば終了 = 押しっぱなしで進まないようにする
			while (!(!prevTouch && IsTouched))
			{
				prevTouch = IsTouched;
				yield return null;
			}
		}
	}
}