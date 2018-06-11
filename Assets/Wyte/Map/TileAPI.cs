using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Novel.Exceptions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAPI : SingletonBaseBehaviour<TileAPI>
{
	[System.Serializable]
	public class TileKeyValuePair
	{
		public string Id;
		public TileBase Tile;
	}

	[SerializeField]
	private TileKeyValuePair[] tiles;

	Tilemap tilemap;

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if (tilemap == null)
		{
			tilemap = FindObjectOfType<Tilemap>();
		}
	}

	public TileBase GetRegisteredTile(string id) => tiles.FirstOrDefault(t => t.Id == id)?.Tile;

	public void Place(string id, Vector2 pos)
	{
		var tile = GetRegisteredTile(id);

		if (tile != null)
			Place(tile, pos);
	}

	public void Place(TileBase tile, Vector2 pos)
	{
		if (tilemap == null)
			return;

		tilemap.SetTile(Vector3Int.FloorToInt(pos), tile);
	}

	public void Delete(Vector2 pos)
	{
		Place(default(TileBase), pos);
	}

	public bool Exists(string id, Vector2 pos)
	{
		if (tilemap == null)
			return false;

		return Get(pos) == GetRegisteredTile(id);
	}

	public TileBase Get(Vector2 pos)
	{
		if (tilemap == null)
			return null;

		return tilemap.GetTile(Vector3Int.FloorToInt(pos));
	}

	//+ontile <id>, <x>, <y>, <goto/gosub>, <#yes>, [#no]
	public IEnumerator OnTile(string _, params string[] args)
	{
		NArgsAssert(args.Length >= 5);
		string id = args[0];
		int x, y;
		NArgsAssert(int.TryParse(args[1], out x), 1);
		NArgsAssert(int.TryParse(args[2], out y), 2);
		var goFlag = args[3].ToLower();
		NArgsAssert(goFlag == "goto" || goFlag == "gosub", 3);
		var yes = args[4];
		var no = args.Length >= 6 ? args[5] : null;

		var labelToGo = Exists(id, new Vector2(x, y)) ? yes : no;

		if (labelToGo == null)
			yield break;
		
		if (goFlag == "goto")
		{
			yield return Novel.Runtime.Goto(null, labelToGo);
		}
		else
		{
			yield return Novel.Runtime.Gosub(null, labelToGo);
		}
	}

	// +place <id>, <x>, <y>
	public IEnumerator Place(string _, params string[] args)
	{
		NArgsAssert(args.Length >= 3);

		string id = args[0];
		int x, y;
		NArgsAssert(int.TryParse(args[1], out x), 1);
		NArgsAssert(int.TryParse(args[2], out y), 2);

		Place(id, new Vector2(x, y));
		yield break;
	}

	// +delete <x>, <y>
	internal IEnumerator Delete(string _, params string[] args)
	{
		int x, y;
		NArgsAssert(int.TryParse(args[0], out x), 0);
		NArgsAssert(int.TryParse(args[1], out y), 1);
		Delete(new Vector2(x, y));

		yield break;
	}
}
