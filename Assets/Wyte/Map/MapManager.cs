using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using WyteEngine.Event;

namespace WyteEngine.Map
{
	public class MapManager : SingletonBaseBehaviour<MapManager>
	{
		public MapPropertyBase[] Maps;

		private GameObject currentMapObject;

		public MapPropertyBase CurrentMap { get; private set; }
		public Rect CurrentMapSize { get; private set; }

		private void Start()
		{
			Wyte.GameReset += (wyte) =>
			{
				Unload();
			};
			Debugger.DebugRendering += (d) => d.Append($"map:{CurrentMap?.name ?? "NULL"} msz{CurrentMapSize.min},{CurrentMapSize.max} ");
		}

		public void Move(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				Unload();

				GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = Color.black;
				MapChanged?.Invoke(null);
				return;
			}

			var map = Maps.FirstOrDefault(m => m.gameObject.name == name);

			if (map == null)
				throw new ArgumentException($"{name} というマップが見つかりませんでした．");

			Unload();

			currentMapObject = Instantiate(map.gameObject) as GameObject;
			CurrentMap = map;
			CurrentMap.Initialize(this);


			CurrentMapSize = CurrentMap.Size;

			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = map.BackColor;
			MapChanged?.Invoke(map);
		}

		public void Unload()
		{
			if (currentMapObject)
				Destroy(currentMapObject);
			currentMapObject = null;
		}

		public IEnumerator Move(string _, params string[] args)
		{
			Move(NovelHelper.CombineAll(args));
			yield break;
		}

		public delegate void MapChangedEventHandler(MapPropertyBase map);
		public event MapChangedEventHandler MapChanged;
	}
}