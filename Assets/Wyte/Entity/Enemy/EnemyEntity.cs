using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : NpcBehaviour 
{
	protected new BoxCollider2D collider2D;
	protected BoxCollider2D playerCollider;

	protected override void OnUpdate()
	{
		CheckCollision();
	}

	protected override void CheckCollision()
	{
		
	}
}