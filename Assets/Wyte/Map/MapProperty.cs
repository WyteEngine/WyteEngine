using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WyteEngine.Map
{
	/// <summary>
	/// 標準のタイルマップ用プロパティです．
	/// </summary>
	public class MapProperty : MapPropertyBase
	{
		public override void Initialize(MapManager m)
		{
			var tmaps = gameObject.GetComponentsInChildren<Tilemap>();
			var cs = gameObject.GetComponent<Grid>().cellSize;
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
			Size = rect;
		}
	}
}