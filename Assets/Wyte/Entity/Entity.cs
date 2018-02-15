using System.Collections;
using UnityEngine;
using System;
using UObject = UnityEngine.Object;
/// <summary>
/// Wyte のマップを移動可能なオブジェクトです．
/// </summary>
public abstract class Entity : BaseBehaviour
{
	[Header("Event")]
	[SerializeField]
	protected string spriteTag;

	/// <summary>
	/// このEntityのタグを取得または設定します．
	/// </summary>
	public string Tag
	{
		get { return spriteTag; }
		set { spriteTag = value; }
	}

	[SerializeField]
	protected string flagId;

	/// <summary>
	/// 表示設定の元になるフラグIDを取得または設定します．．
	/// </summary>
	/// <value>The flag identifier.</value>
	public string FlagId
	{
		get { return flagId; }
		set { flagId = value; }
	}

	[SerializeField]
	private EntityVisiblity visiblity;

	public EntityVisiblity Visiblity
	{
		get { return visiblity; }
		set { visiblity = value; }
	}

	public bool Dead { get; protected set; }
	public virtual int MaxHealth => 1;
		public int Health { get; protected set; }

	/// <summary>
	/// 死んでいる途中であるかどうか取得します．
	/// </summary>
	/// <value>死んでいる途中であれば<c>true</c>，違えば<c>false</c>．</value>
	public bool Dying { get; protected set; }

	protected virtual void Start()
	{
		Health = MaxHealth;
		if (Health == 0)
			Kill(null);
		switch (Visiblity)
		{
			case EntityVisiblity.Visible:
				// Nothing to do.
				break;
			case EntityVisiblity.InVisible:
				Destroy(this);
				break;
			case EntityVisiblity.VisibleWhenOn:
				if (!Flag.Flags[FlagId])
					Destroy(this);
				break;
			case EntityVisiblity.VisibleWhenOff:
				if (Flag.Flags[FlagId])
					Destroy(this);
				break;
			default:
				throw new InvalidOperationException("予期しない EntityVisiblity．");
		}
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

	protected virtual IEnumerator OnDeath(UObject killer)
	{
		yield break;
	}

	protected virtual IEnumerator OnDamaged(UObject interacter)
	{
		yield break;
	}

	protected virtual IEnumerator OnHealed(UObject interacter)
	{
		yield break;
	}

	public void Damage(UObject interacter, int atk)
	{
		Health -= atk;
		if (Health < 0)
		{
			Health = 0;
			// 殺人容疑
			Kill(interacter);
		}
	}

	public void Heal(UObject interacter, int point)
	{
		Health += point;
		if (Health > MaxHealth)
			Health = MaxHealth;
	}

	public void Kill(UObject killer)
	{
		StartCoroutine(Death(killer));
	}

	IEnumerator Death(UObject killer)
	{
		Dying = true;
		yield return OnDeath(killer);
		Dead = true;
	}
}

public enum EntityVisiblity
{
	/// <summary>
	/// 常に表示
	/// </summary>
	Visible,
	/// <summary>
	/// 常に非表示
	/// </summary>
	InVisible,
	/// <summary>
	/// フラグがオンなら表示
	/// </summary>
	VisibleWhenOn,
	/// <summary>
	/// フラグがオフなら表示
	/// </summary>
	VisibleWhenOff
}