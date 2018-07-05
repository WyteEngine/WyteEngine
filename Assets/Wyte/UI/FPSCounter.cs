using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
namespace WyteEngine.UI
{

	public class FPSCounter : SingletonBaseBehaviour<FPSCounter>
	{

		int cnt;
		float time;

		StringBuilder debugTexts;

		public delegate void DebugRenderingEventHandler(StringBuilder debugTexts);
		public event DebugRenderingEventHandler DebugRendering;

		bool isDebugMode;
		int fps;

		protected override void Awake()
		{
			base.Awake();
			debugTexts = new StringBuilder();
		}

		// Use this for initialization
		void Start()
		{
			DebugRendering += (d) =>
			{
				d.Append(FpsString);
			};
		}

		// Update is called once per frame
		protected override void Update()
		{
			base.Update();
			if (Input.GetKeyDown(KeyCode.F3))
			{
				isDebugMode = !isDebugMode;

				Debug.Log($"<color=yellow>デバッグ情報表示を{(isDebugMode ? "有効化" : "無効化")}しました．</color>", this);
			}

			string output = "";
			if (isDebugMode)
			{
				debugTexts.Clear();
				DebugRendering?.Invoke(debugTexts);
				output = debugTexts.ToString();
			}
			else
			{
				// Non-debug-mode fps display has no longer supported
				output = "";
			}
			GetComponent<Text>().text = output;

			if (time > 1)
			{
				fps = cnt;
				cnt = 0;
				time = 0;
				return;
			}
			cnt++;
			time += Time.deltaTime;
		}

		string FpsString => $"fps{fps} ";
	}
}