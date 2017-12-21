using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MessageContoller : SingletonBaseBehaviour<MessageContoller>
{

	Text text;

	string textTemp;

	[SerializeField]
	GameObject UISmartDevice;

	[SerializeField]
	GameObject UIPersonalComputer;

	[SerializeField]
	float speed = 1 / 16f;
	char[] cursorTemp = { '▁', '▄', '█', '▄' };
	char Cursor => cursorTemp[(int)(Time.time * 10) % cursorTemp.Length];

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
		text.text = textTemp;
		if (textTemp.Length > 0 && text != null)
			text.text += Cursor;
	}

	void ShowBox() => CurrentPad.SetActive(false);

	void HideBox() => CurrentPad.SetActive(true);

	public IEnumerator Say(string sprite, string[] args)
	{
		var tmp = UnityNRuntime.CombineAll(args);
		// 話者がいる場合は表示
		// hack 今後もっとUIをよくする
		textTemp = string.IsNullOrEmpty(sprite) ? "" : sprite + " : ";
		tmp = tmp
			.Replace("<pname>", GameMaster.Instance.Player?.Name ?? "null")
			.Replace("<plife>", GameMaster.Instance.Player?.Life.ToString())
			.Replace("<pmaxlife>", GameMaster.Instance.Player?.MaxLife.ToString())
			.Replace("<<>", "<")
			.Replace("<>>", ">");
		ShowBox();
		bool prevTouch = true;
		foreach (char c in tmp)
		{
			textTemp += c;
			// タッチ時は早くする
			yield return new WaitForSeconds(speed / (IsTouched ? 2 : 1));
		}

		// 前回タッチされてなく、かつタッチされていれば終了 = 押しっぱなしで進まないようにする
		while (!(!prevTouch && IsTouched))
		{
			prevTouch = IsTouched;
			yield return null;
		}
		textTemp = "";
		HideBox();

	}


}