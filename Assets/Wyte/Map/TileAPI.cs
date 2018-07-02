using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Novel.Exceptions;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace WyteEngine.Map
{

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

		Tilemap[] tilemaps;

		Tilemap Tilemap => tilemaps != null && tilemaps.Length > 0 ? tilemaps[0] : null;

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		void Update()
		{
			if (Tilemap == null)
			{
				tilemaps = FindObjectsOfType<Tilemap>();
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
			if (Tilemap == null)
				return;

			Tilemap.SetTile(Vector3Int.FloorToInt(pos), tile);
		}

		public void Delete(Vector2 pos)
		{
			foreach (var t in tilemaps)
			{
				t.SetTile(Vector3Int.FloorToInt(pos), null);
			}
		}

		public bool Exists(string id, Vector2 pos)
		{
			if (Tilemap == null)
				return false;

			return Get(pos) == GetRegisteredTile(id);
		}

		public TileBase Get(Vector2 pos)
		{
			if (Tilemap == null)
				return null;

			return Tilemap.GetTile(Vector3Int.FloorToInt(pos));
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

		// +tileset <id>, <x>, <y>
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

		// +tiledel <x>, <y>
		internal IEnumerator Delete(string _, params string[] args)
		{
			int x, y;
			NArgsAssert(int.TryParse(args[0], out x), 0);
			NArgsAssert(int.TryParse(args[1], out y), 1);
			Delete(new Vector2(x, y));

			yield break;
		}

		// +tilesetrect <id>, <x1>, <y1>, <x2>, <y2>
		public IEnumerator PlaceRect(string _, params string[] args)
		{
			NArgsAssert(args.Length >= 3);

			string id = args[0];
			int x1, y1, x2, y2;
			NArgsAssert(int.TryParse(args[1], out x1), 1);
			NArgsAssert(int.TryParse(args[2], out y1), 2);
			NArgsAssert(int.TryParse(args[3], out x2), 3);
			NArgsAssert(int.TryParse(args[4], out y2), 4);

			if (y2 < y1)
			{
				var t = y2;
				y2 = y1;
				y1 = t;
			}

			if (x2 < x1)
			{
				var t = x2;
				x2 = x1;
				x1 = t;
			}

			for (int y = y1; y <= y2; y++)
				for (int x = x1; x <= x2; x++)
					Place(id, new Vector2(x, y));

			yield break;
		}

		// +tiledelrect <x1>, <y1>, <x2>, <y2>
		internal IEnumerator DeleteRect(string _, params string[] args)
		{
			int x1, y1, x2, y2;
			NArgsAssert(int.TryParse(args[0], out x1), 0);
			NArgsAssert(int.TryParse(args[1], out y1), 1);
			NArgsAssert(int.TryParse(args[2], out x2), 2);
			NArgsAssert(int.TryParse(args[3], out y2), 3);

			if (y2 < y1)
			{
				var t = y2;
				y2 = y1;
				y1 = t;
			}

			if (x2 < x1)
			{
				var t = x2;
				x2 = x1;
				x1 = t;
			}

			for (int y = y1; y <= y2; y++)
				for (int x = x1; x <= x2; x++)
					Delete(new Vector2(x, y));

			yield break;
		}
	}
}