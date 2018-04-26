using UnityEngine;

public abstract class AIBaseBehaviour : BaseBehaviour
{
	public AINodeBase OnUpdate { get; protected set; }
	
	public AINodeBase OnCollidedWithPlayer { get; protected set; }

	protected virtual void Awake()
	{
		OnInitialize();
	}

	protected virtual void OnInitialize() { }
}