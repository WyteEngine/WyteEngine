// コードは以下のURLから拝借しました。
// http://qiita.com/ruccho_vector/items/fcd8ecea1538d0864283

using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UnityEngine.Tilemaps
{
	[Serializable]
	public class WolfAutoTile : TileBase
	{
		[SerializeField]
		public Sprite[] m_RawTilesSprites;
		//All 47 Patterns
		public Sprite[] m_PatternedSprites;

		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
		{
			for (var yd = -1; yd <= 1; yd++)
				for (var xd = -1; xd <= 1; xd++)
				{
					var position = new Vector3Int(location.x + xd, location.y + yd, location.z);
					if (TileValue(tileMap, position))
						tileMap.RefreshTile(position);
				}
		}

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			UpdateTile(location, tileMap, ref tileData);
			return;
		}

		private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			if (m_PatternedSprites[0] == null)
			{
				if (m_RawTilesSprites[0] && m_RawTilesSprites[1] && m_RawTilesSprites[2] && m_RawTilesSprites[3] && m_RawTilesSprites[4])
				{
					GeneratePatterns();
				}
				else
				{
					return;
				}
			}
			tileData.transform = Matrix4x4.identity;
			tileData.color = Color.white;

			var mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;
			var index = GetIndex((byte)mask);
			if (index >= 0 && index < m_PatternedSprites.Length && TileValue(tileMap, location))
			{
				tileData.sprite = m_PatternedSprites[index];
				tileData.color = Color.white;
				tileData.flags = (TileFlags.LockTransform | TileFlags.LockColor);
				tileData.colliderType = Tile.ColliderType.Sprite;
			}
		}

		private bool TileValue(ITilemap tileMap, Vector3Int position)
		{
			var tile = tileMap.GetTile(position);
			return (tile != null && tile == this);
		}

		private int GetIndex(byte mask)
		{
			string[] patternTexts = {
				"x0x111x0",
				"x11111x0",
				"x111x0x0",
				"x10111x0",
				"x11101x0",
				"01111111",
				"11111101",
				"x0x1x0x0",
				"x0x11111",
				"11111111",
				"1111x0x1",
				"x0x10111",
				"1101x0x1",
				"11011111",
				"11110111",
				"x0x1x0x1",
				"x0x0x111",
				"11x0x111",
				"11x0x0x1",
				"x0x11101",
				"0111x0x1",
				"01110111",
				"11011101",
				"x0x0x0x1",
				"x0x101x0",
				"x10101x0",
				"x101x0x0",
				"01x0x111",
				"11x0x101",
				"11010101",
				"01010111",
				"11010111",
				"x0x10101",
				"01010101",
				"0101x0x1",
				"11110101",
				"01011111",
				"01110101",
				"01011101",
				"01111101",
				"x0x0x101",
				"01x0x101",
				"01x0x0x1",
				"x0x0x1x0",
				"x1x0x1x0",
				"x1x0x0x0",
				"x0x0x0x0"
			};
			var index = -1;
			for (var j = 0; j < patternTexts.Length; j++)
			{
				var flag = true;
				for (var i = 0; i < 8; i++)
				{

					if (patternTexts[j][i] != 'x')
					{
						var currentBitChar = ((mask & (byte)Mathf.Pow(2, 7 - i)) != 0) ? '1' : '0';
						if (patternTexts[j][i] != currentBitChar)
						{

							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					index = j;
					break;
				}
			}
			return index;


		}



		Sprite[,] Segments = new Sprite[5, 4];
		int[][] Patterns = new int[][]
		{
			new int[] {0,2,1,4},
			new int[] {2,2,4,4},
			new int[] {2,0,4,1},
			new int[] {2,2,3,4},
			new int[] {2,2,4,3},
			new int[] {3,4,4,4},
			new int[] {4,3,4,4},
			new int[] {0,0,1,1},
			new int[] {1,4,1,4},
			new int[] {4,4,4,4},
			new int[] {4,1,4,1},
			new int[] {1,4,1,3},
			new int[] {4,1,3,1},
			new int[] {4,4,3,4},
			new int[] {4,4,4,3},
			new int[] {1,1,1,1},

			new int[] {1,4,0,2},
			new int[] {4,4,2,2},
			new int[] {4,1,2,0},
			new int[] {1,3,1,4},
			new int[] {3,1,4,1},
			new int[] {3,4,4,3},
			new int[] {4,3,3,4},
			new int[] {1,1,0,0},

			new int[] {0,2,1,3},
			new int[] {2,2,3,3},
			new int[] {2,0,3,1},
			new int[] {3,4,2,2},
			new int[] {4,3,2,2},
			new int[] {4,3,3,3},
			new int[] {3,4,3,3},
			new int[] {4,4,3,3},

			new int[] {1,3,1,3},
			new int[] {3,3,3,3},
			new int[] {3,1,3,1},
			new int[] {4,3,4,3},
			new int[] {3,4,3,4},
			new int[] {3,3,4,3},
			new int[] {3,3,3,4},
			new int[] {3,3,4,4},

			new int[] {1,3,0,2},
			new int[] {3,3,2,2},
			new int[] {3,1,2,0},
			new int[] {0,2,0,2},
			new int[] {2,2,2,2},
			new int[] {2,0,2,0},
			new int[] {0,0,0,0}

		};
		public void GeneratePatterns()
		{

			for (var i = 0; i < 5; i++)
			{
				var tex = m_RawTilesSprites[i].texture;
				var y = (int)m_RawTilesSprites[i].rect.y;
				var x = (int)m_RawTilesSprites[i].rect.x;
				var height = (int)m_RawTilesSprites[i].rect.height;
				var width = (int)m_RawTilesSprites[i].rect.width;
				var height_half = height / 2;
				var width_half = width / 2;
				Segments[i, 0] = Sprite.Create(tex, new Rect(x, y, width_half, height_half), Vector2.zero);
				Segments[i, 1] = Sprite.Create(tex, new Rect(x + width_half, y, width_half, height_half), Vector2.zero);
				Segments[i, 2] = Sprite.Create(tex, new Rect(x, y + height_half, width_half, height_half), Vector2.zero);
				Segments[i, 3] = Sprite.Create(tex, new Rect(x + width_half, y + height_half, width_half, height_half), Vector2.zero);

			}

			m_PatternedSprites = new Sprite[47];
			for (var i = 0; i < 47; i++)
			{
				m_PatternedSprites[i] = CombineTextures(Patterns[i]);
			}
		}

		private Sprite CombineTextures(int[] TypeIndex)
		{
			var fixedArray = new int[4];
			fixedArray[0] = TypeIndex[2];
			fixedArray[1] = TypeIndex[3];
			fixedArray[2] = TypeIndex[0];
			fixedArray[3] = TypeIndex[1];



			var texs = new Color[4][];
			for (var i = 0; i < 4; i++)
			{

				var x = (int)Segments[fixedArray[i], i].rect.x;
				var y = (int)Segments[fixedArray[i], i].rect.y;
				var w = (int)Segments[fixedArray[i], i].rect.width;
				var h = (int)Segments[fixedArray[i], i].rect.height;
				texs[i] = Segments[fixedArray[i], i].texture.GetPixels(x, y, w, h);
			}

			var width_half = (int)Segments[0, 0].rect.width;
			var height_half = (int)Segments[0, 0].rect.height;
			var width = width_half * 2;
			var height = height_half * 2;

			var texArray = new Color[width * height];
			for (var i = 0; i < height_half; i++)
			{

				Array.Copy(texs[0], i * width_half, texArray, i * width, width_half);
			}

			for (var i = 0; i < height_half; i++)
			{
				Array.Copy(texs[1], i * width_half, texArray, i * width + width_half, width_half);
			}

			for (var i = 0; i < height_half; i++)
			{
				Array.Copy(texs[2], i * width_half, texArray, (i + height_half) * width, width_half);
			}

			for (var i = 0; i < height_half; i++)
			{
				Array.Copy(texs[3], i * width_half, texArray, (i + height_half) * width + width_half, width_half);
			}
			var ret = new Texture2D(width, height, TextureFormat.ARGB32, false)
			{ filterMode = FilterMode.Point};
			ret.SetPixels(texArray);

			return Sprite.Create(ret, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), m_RawTilesSprites[0].pixelsPerUnit);
		}



#if UNITY_EDITOR
		[MenuItem("Assets/Create/WolfAuto Tile")]
		public static void CreateTerrainTile()
		{
			var path = EditorUtility.SaveFilePanelInProject("Save WolfAuto Tile", "New WolfAuto Tile", "asset", "Save WolfAuto Tile", "Assets");

			if (path == "")
				return;

			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WolfAutoTile>(), path);
		}
#endif
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(WolfAutoTile))]
	public class WolfAutoTileEditor : Editor
	{
		private WolfAutoTile Tile { get { return (target as WolfAutoTile); } }

		public void OnEnable()
		{
			if (Tile.m_RawTilesSprites == null || Tile.m_RawTilesSprites.Length != 15)
			{
				Tile.m_RawTilesSprites = new Sprite[15];
				EditorUtility.SetDirty(Tile);
			}
		}


		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField("ウディタ規格のオートタイルチップを上から順番にスロットしてください。（アニメーション非対応）");
			EditorGUILayout.Space();

			var oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 210;

			EditorGUI.BeginChangeCheck();
			Tile.m_RawTilesSprites[0] = (Sprite)EditorGUILayout.ObjectField("上下左右どこにもこのオートタイルがない(直角に配置したときの外側の角に現れる)", Tile.m_RawTilesSprites[0], typeof(Sprite), false, null);
			Tile.m_RawTilesSprites[1] = (Sprite)EditorGUILayout.ObjectField("上下にこのオートタイルがある", Tile.m_RawTilesSprites[1], typeof(Sprite), false, null);
			Tile.m_RawTilesSprites[2] = (Sprite)EditorGUILayout.ObjectField("左右にこのオートタイルがある", Tile.m_RawTilesSprites[2], typeof(Sprite), false, null);
			Tile.m_RawTilesSprites[3] = (Sprite)EditorGUILayout.ObjectField("直角にこのオートタイルを配置したとき、内側の角に現れる", Tile.m_RawTilesSprites[3], typeof(Sprite), false, null);
			Tile.m_RawTilesSprites[4] = (Sprite)EditorGUILayout.ObjectField("このオートタイルに周囲を全て囲まれている", Tile.m_RawTilesSprites[4], typeof(Sprite), false, null);

			if (EditorGUI.EndChangeCheck())
			{

				if (Tile.m_RawTilesSprites[0] && Tile.m_RawTilesSprites[1] && Tile.m_RawTilesSprites[2] && Tile.m_RawTilesSprites[3] && Tile.m_RawTilesSprites[4])
				{
					Tile.GeneratePatterns();
				}

				EditorUtility.SetDirty(Tile);
			}

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
	}
#endif
}
