using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Novel.Exceptions;
using System.Security.AccessControl;

[RequireComponent(typeof(Image))]
public class FadeController : SingletonBaseBehaviour<FadeController> {
	
	Image image;

	private void Start()
	{
		image = GetComponent<Image>();
	}

	Color light = new Color(0, 0, 0, 0);
	Color dark = new Color(0, 0, 0, 1);

	public IEnumerator Fade(string _, params string[] args)
	{
		if (image == null)
			throw new InvalidOperationException("bug: the image is null.");
		
		string mode = "in";
		float time = 1;
		float nowTime = 0;
		if (args.Length >= 2)
		{
			if (!float.TryParse(args[1], out time))
				throw new NRuntimeException("時間が不正です。");
			mode = args[0];
		}
		else if (args.Length >= 1)
		{
			if (!float.TryParse(args[0], out time))
				throw new NRuntimeException("時間が不正です。");
		}

		while (nowTime < time)
		{
			switch (mode.ToLower())
			{
				case "in":
					image.color = Color.Lerp(dark, light, nowTime / time);
					break;
				case "out":
					image.color = Color.Lerp(light, dark, nowTime);
					break;
				default:
					throw new NRuntimeException("モードがおかしいです。");
			}
			nowTime += Time.deltaTime;
			yield return null;
		}
		switch (mode.ToLower())
		{
			case "in":
				image.color = light;
				break;
			case "out":
				image.color = dark;
				break;
		}
	}
}
