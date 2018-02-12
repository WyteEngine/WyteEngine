using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class ScreenShotHandler : SingletonBaseBehaviour<ScreenShotHandler>
{
	private static readonly string dest = "Screenshots";

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2))
		{
			var file = DateTime.Now.ToString("yyMMdd-hhmmss") + ".png";
			if (IsSmartDevice)
				ScreenCapture.CaptureScreenshot(file);
			else
			{
				StartCoroutine(TakeShot(file));
			}
		}
	}

	private IEnumerator TakeShot(string file)
	{
		yield return new WaitForEndOfFrame();

		var path = Path.Combine(Environment.CurrentDirectory, dest);
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
		path = Path.Combine(path, file);
		var tex = ScreenCapture.CaptureScreenshotAsTexture();
		File.WriteAllBytes(path, tex.EncodeToPNG());
		Destroy(tex);
	}
}