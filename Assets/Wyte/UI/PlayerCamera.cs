using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Novel.Exceptions;
using System;
using static NovelHelper;

public class PlayerCamera : SingletonBaseBehaviour<PlayerCamera>
{

	public Transform player;

	private static readonly Vector3 zero = new Vector3(0, 0, -1);

	private Vector3 offset = zero;

	public CameraTarget Target { get; set; } = CameraTarget.Player;

	public Vector3 FreePosition { get; private set; }

	
	private Camera theCamera;

	private Vector3 camMin, camMax;

	public Rect CameraRects => new Rect((camMin + camMax) / 2, camMax - camMin);

	void Start()
	{
		Wyte.GameReset += (wyte) =>
		{
			offset = zero;
			player = null;
		};

		Map.MapChanged += (m) =>
		{
			offset = zero;
			player = null;
		};
		theCamera = GetComponent<Camera>();
		Debugger.DebugRendering += (d) =>
		{
			d.Append($"cam{camMin},{camMax} ");
		};
	}

	void LateUpdate()
	{
		var z = transform.position.z;
		if (!player)
		{
				
			var g = GameObject.FindGameObjectWithTag("Player");
			if (!g) return;
			player = g.transform;
			transform.position = player.position + Vector3.back;
			offset = transform.position - player.position;
		}

		var newPosition = transform.position;

		switch (Target)
		{
			case CameraTarget.Player:
				if (!player.GetComponent<PlayerController>().Dying && player.gameObject != null)
					newPosition = player.position + offset + Vector3.down * 24;
				break;
			case CameraTarget.Free:
				newPosition = FreePosition;
				break;
		}

		camMin = theCamera.ViewportToWorldPoint(Vector3.zero);
		camMax = theCamera.ViewportToWorldPoint(Vector3.one);
		var camSize = camMax - camMin;

		if (Map.CurrentMapSize != Rect.zero)
		{
			// 座標候補における左端(中心-全体サイズ/2) が，マップの左端より小さくなったらスナップする
			if (newPosition.x - camSize.x / 2 < Map.CurrentMapSize.xMin)
				newPosition.x = Map.CurrentMapSize.xMin + camSize.x / 2;

			// 座標候補における右端(中心+全体サイズ/2) が，マップの右端より大きくなったらスナップする
			if (newPosition.x + camSize.x / 2 > Map.CurrentMapSize.xMax)
				newPosition.x = Map.CurrentMapSize.xMax - camSize.x / 2;

			// 座標候補における上端(中心-全体サイズ/2) が，マップの上端より小さくなったらスナップする
			if (newPosition.y - camSize.y / 2 < Map.CurrentMapSize.yMin)
				newPosition.y = Map.CurrentMapSize.yMin + camSize.y / 2;

			// 座標候補における下端(中心+全体サイズ/2) が，マップの下端より大きくなったらスナップする
			if (newPosition.y + camSize.y / 2 > Map.CurrentMapSize.yMax)
				newPosition.y = Map.CurrentMapSize.yMax - camSize.y / 2;
		}
		transform.position = Vector3.Lerp(transform.position, newPosition, 5f * Time.deltaTime);
		transform.position.Set(transform.position.x, transform.position.y, z);
	}

	public IEnumerator SwitchToPlayerCamera(string _, string[] args)
	{
		SwitchToPlayerCamera();
		yield break;
	}

	public bool IsVisible(Vector3 vec)
	{
		var min = camMin - (camMax - camMin) / 2;
		var max = camMax + (camMax - camMin) / 2;
		return min.x < vec.x && vec.x < max.x && min.y < vec.y && vec.y < max.y;
	}


	public IEnumerator SwitchToFreeCamera(string _, string[] args)
	{
		if (args.Length < 2)
			throw new NRuntimeException("引数が足りません．");
		var x = TryParse(args[0]);
		var y = TryParse(args[1]);
		SwitchToFreeCamera(x, y);
		yield break;
	}

	/// <summary>
	/// プレイヤーカメラに切り替えます．
	/// </summary>
	public void SwitchToPlayerCamera()
	{
		Target = CameraTarget.Player;
	}

	/// <summary>
	/// 座標を指定してフリーカメラに切り替えます．
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SwitchToFreeCamera(float x, float y)
	{
		FreePosition = new Vector3(x, y, -1);
		Target = CameraTarget.Free;
	}

}



public enum CameraTarget
{
	Player,
	Free
}
