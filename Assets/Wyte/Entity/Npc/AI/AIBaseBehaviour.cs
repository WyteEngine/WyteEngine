using UnityEngine;

public abstract class AIBaseBehaviour : BaseBehaviour
{
	public virtual AINodeBase OnUpdate { get; }
	
	public virtual AINodeBase OnCollidedWithPlayer { get; }
}