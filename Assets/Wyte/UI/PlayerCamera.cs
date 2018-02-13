using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Novel.Exceptions;
using System;

public class PlayerCamera : SingletonBaseBehaviour<PlayerCamera>
{

	public Transform player;

	private Vector3 offset = Vector3.zero;

	public CameraTarget Target { get; set; } = CameraTarget.Player;

	public Vector3 FreePosition { get; private set; }

	private RectTransform windowRect;

	void Start()
	{
		if (windowRect == null)
			windowRect = GameObject.FindGameObjectWithTag("UIWindow").GetComponent<RectTransform>();
		WyteEvent.Instance.GameReset += (wyte) =>
		{
			offset = Vector3.zero;
			player = null;
		};

		WyteEvent.Instance.MapChanged += (m) =>
		{
			offset = Vector3.zero;
			player = null;
		};
	}

	void LateUpdate()
	{
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
				newPosition = player.position + offset;
				break;
			case CameraTarget.Free:
				newPosition = FreePosition;
				break;
		}

		transform.position = Vector3.Lerp(transform.position, newPosition, 5f * Time.deltaTime);
	}

	private float TryParse(string numeric)
	{
		float ret;
		if (!float.TryParse(numeric, out ret))
			throw new NRuntimeException("型が一致しません．");
		return ret;
	}

	public IEnumerator SwitchToPlayerCamera(string _, string[] args)
	{
		SwitchToPlayerCamera();
		yield break;
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
