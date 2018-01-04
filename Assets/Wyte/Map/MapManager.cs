using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class MapManager : SingletonBaseBehaviour<MapManager>
{
	public MapProperty[] Maps;

	private GameObject currentMapObject;

	public MapProperty CurrentMap { get; private set; }

	private void Start()
	{
		WyteEvent.Instance.GameReset += (wyte) =>
		{
			Unload();
		};
	}

	public void Move(string name)
	{
		var map = Maps.FirstOrDefault(m => m.gameObject.name == name);

		if (map == null) 
			throw new ArgumentException($"{name} というマップが見つかりませんでした．");

		if (currentMapObject)
			Destroy(currentMapObject);
		currentMapObject = Instantiate(map.gameObject) as GameObject;
		CurrentMap = map;
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = map.BackColor;
	}

	public void Unload()
	{
		if (currentMapObject)
			Destroy(currentMapObject);
	}

	public IEnumerator Move(string _, params string[] args)
	{
		Move(UnityNRuntime.CombineAll(args));
		yield break;
	}
}
