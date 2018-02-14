using System.Collections;
using UnityEngine;
/// <summary>
/// Wyte のマップを移動可能なオブジェクトです．
/// </summary>
public abstract class Entity : BaseBehaviour
{
	/// <summary>
	/// このEntityのタグを取得または設定します．
	/// </summary>
	public string Tag
	{
		get { return spriteTag; }
		set { spriteTag = value; }
	}

	[Header("Event")]
	[SerializeField]
	protected string spriteTag;

	public bool Dead { get; protected set; }
	public virtual int MaxHealth => 1;
	public int Health { get; protected set; }

	/// <summary>
	/// 死んでいる途中であるかどうか取得します．
	/// </summary>
	/// <value>死んでいる途中であれば<c>true</c>，違えば<c>false</c>．</value>
	public bool Dying { get; private set; }

	protected virtual void Start()
	{
		Health = MaxHealth;
	}

	protected void Update()
	{
		// 生きていればアップデート
		if (!Dying)
			OnUpdate();
		// 死んでいれば供養
		if (Dead)
			Destroy(this);
	}

	protected void FixedUpdate()
	{
		// 生きていればアップデート
		if (!Dying)
			OnFixedUpdate();
	}

	/// <summary>
	/// フレーム毎に呼ばれます．
	/// </summary>
	protected virtual void OnUpdate() { }


	/// <summary>
	/// フレーム毎に呼ばれます．
	/// </summary>
	protected virtual void OnFixedUpdate() { }

	protected virtual IEnumerator OnDeath(Object killer)
	{
		yield break;
	}

	protected virtual IEnumerator OnDamaged(Object interacter)
	{
		yield break;
	}

	protected virtual IEnumerator OnHealed(Object interacter)
	{
		yield break;
	}

	public void Damage(Object interacter, int atk)
	{
		Health -= atk;
		if (Health < 0)
		{
			Health = 0;
			// 殺人容疑
			Kill(interacter);
		}
	}

	public void Heal(Object interacter, int point)
	{
		Health += point;
		if (Health > MaxHealth)
			Health = MaxHealth;
	}

	public void Kill(Object killer)
	{
		StartCoroutine(Death(killer));
	}

	IEnumerator Death(Object killer)
	{
		Dying = true;
		yield return OnDeath(killer);
		Dead = true;
	}
}
