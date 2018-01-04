using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : BaseBehaviour
{

	public Transform player;

	private Vector3 offset = Vector3.zero;

	public CameraTarget Target { get; set; } = CameraTarget.Player;

	public Vector3 FreePosition { get; set; }

	[SerializeField]
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
	}

	void LateUpdate()
	{
		if (!player)
		{
			var g = GameObject.FindGameObjectWithTag("Player");
			if (!g) return;
			player = g.transform;
			transform.position = new Vector3(0, 0, -1);
			offset = Vector3.zero;
		}
		if (offset == Vector3.zero)
		{
			offset = transform.position - player.transform.position;
		}
		

		var newPosition = transform.position;

		switch (Target)
		{
			case CameraTarget.Player:
				newPosition.x = player.transform.position.x + offset.x;
				newPosition.y = player.transform.position.y + offset.y - windowRect.rect.height;
				//if (newPosition.y < )
				newPosition.z = player.transform.position.z + offset.z;
				break;
			case CameraTarget.Free:
				newPosition = FreePosition;
				break;
		}

		transform.position = Vector3.Lerp(transform.position, newPosition, 10.0f * Time.deltaTime);
	}
}



public enum CameraTarget
{
	Player,
	Free
}
