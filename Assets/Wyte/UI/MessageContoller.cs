using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Text))]
public class MessageContoller : SingletonBaseBehaviour<MessageContoller>
{

	Text text;

	string textTemp;

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
		textTemp = "";
	}

	bool IsTouched => IsSmartDevice
					? (Input.touchCount > 0)
					: (Input.GetKey(KeyBind.Jump));

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

		text.text = textTemp;
		if (textTemp.Length > 0 && text != null)
			text.text += Cursor;
	}

	public void HideBox() => CurrentPad.SetActive(false);

	public void ShowBox() => CurrentPad.SetActive(true);

	public IEnumerator Say(string sprite, params string[] args)
	{
		var tmp = NovelHelper.CombineAll(args);
		// 話者がいる場合は表示
		// hack 今後もっとUIをよくする
		textTemp = string.IsNullOrEmpty(sprite) ? "" : sprite + " : ";
	
		bool prevTouch = true;
		foreach (char c in tmp)
		{
			textTemp += c;
			if (!quickEnabled)
			{
				Sfx.Play("entity.npc.saying");
				// タッチ時は早くする
				yield return new WaitForSeconds(speed / (IsTouched ? 2 : 1));
			}
		}

		// 前回タッチされてなく、かつタッチされていれば終了 = 押しっぱなしで進まないようにする
		while (!(!prevTouch && IsTouched))
		{
			prevTouch = IsTouched;
			yield return null;
		}
		textTemp = "";

	}


}