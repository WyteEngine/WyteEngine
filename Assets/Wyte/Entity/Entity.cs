using System.Collections;
using UnityEngine;
/// <summary>
/// Wyte のマップを移動可能なオブジェクトです．
/// </summary>
public abstract class Entity : BaseBehaviour
{
	public bool Dead { get; protected set; }
	public virtual int MaxHealth => 1;
	public int Health { get; protected set; }

	protected virtual void Start()
	{
		Health = MaxHealth;
	}

	protected virtual void Update()
	{
		if (Dead)
			Destroy(this);
	}

	protected virtual IEnumerator OnDeath(Object killer) { yield break; }

	public void Kill(Object killer)
	{
		StartCoroutine(OnDeath(killer));
	}

}
