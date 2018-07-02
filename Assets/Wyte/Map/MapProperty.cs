using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WyteEngine.Map
{

	[System.Serializable]
	public struct Wallpaper
	{
		public Sprite Image;
		public float ScrollSpeed;
		public bool AutoScroll;
	}

	public class MapProperty : MonoBehaviour
	{
		/// <summary>
		/// 背景色．
		/// </summary>
		public Color BackColor;

		/// <summary>
		/// 背景画像．
		/// </summary>
		public Wallpaper Background;

		/// <summary>
		/// 前景画像．
		/// </summary>
		public Wallpaper Foreground;

	}
}