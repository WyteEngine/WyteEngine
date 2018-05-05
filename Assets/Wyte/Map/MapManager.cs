using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : SingletonBaseBehaviour<MapManager>
{
	public MapProperty[] Maps;

	private GameObject currentMapObject;

	public MapProperty CurrentMap { get; private set; }
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
		var tmaps = map.gameObject.GetComponentsInChildren<Tilemap>();
		var cs = map.gameObject.GetComponent<Grid>().cellSize;
		var rect = Rect.zero;
		foreach (var tmap in tmaps)
		{
			tmap.CompressBounds();
			var b = tmap.cellBounds;
			float x = cs.x, y = cs.y;
			var r = Rect.MinMaxRect(b.xMin * x, b.yMin * y, b.xMax * x, b.yMax * y);

			// 取得したものが大きければその分広げる
			rect.xMin = r.xMin < rect.xMin ? r.xMin : rect.xMin;
			rect.yMin = r.yMin < rect.yMin ? r.yMin : rect.yMin;
			rect.xMax = rect.xMax < r.xMax ? r.xMax : rect.xMax;
			rect.yMax = rect.yMax < r.yMax ? r.yMax : rect.yMax;
		}


		CurrentMapSize = rect;

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

	public delegate void MapChangedEventHandler(MapProperty map);
	public event MapChangedEventHandler MapChanged;
}
