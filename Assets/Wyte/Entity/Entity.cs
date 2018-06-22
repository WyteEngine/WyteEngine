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

	public float HealthRatio => Health / (float)MaxHealth;

	/// <summary>
	/// 死んでいる途中であるかどうか取得します．
	/// </summary>
	/// <value>死んでいる途中であれば<c>true</c>，違えば<c>false</c>．</value>
	public bool Dying { get; protected set; }

	/// <summary>
	/// 死亡時に当たり判定がなくなるかどうかを取得します．
	/// </summary>
	/// <value><c>true</c> if destroy collider on dying; otherwise, <c>false</c>.</value>
	public virtual bool DestroyColliderOnDying => true;
	
	private float godTime;

	protected AIBaseBehaviour[] OwnAIs;

	/// <summary>
	/// 残り無敵時間を取得または設定します．0のときは無敵ではありません．
	/// </summary>
	public float GodTime
	{
		get { return godTime; }
		set
		{
			godTime = value < 0 ? 0 : value;
		}
	}

	protected virtual void Start()
	{
		Health = MaxHealth;
		OwnAIs = GetComponents<AIBaseBehaviour>();
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

		foreach (var ai in OwnAIs)
		{
			ai.OnUpdate?.Run(this);
		}

		// 死んでいれば供養
		if (Dead)
			Destroy(this.gameObject);
		GodTime -= Time.deltaTime;
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
		if (GodTime > 0)
			return;
		Health -= atk;
		GodTime = 4;
		if (Health <= 0)
		{
			Health = 0;
			// 殺人容疑
			Kill(interacter);
		}
		else
			OnDamaged(interacter);
	}

	public void Heal(UObject interacter, int point)
	{
		Health += point;
		if (Health > MaxHealth)
			Health = MaxHealth;
		OnHealed(interacter);
	}

	public void Kill(UObject killer)
	{
		StartCoroutine(Death(killer));
	}

	IEnumerator Death(UObject killer)
	{
		Dying = true;

		if (DestroyColliderOnDying)
		{
			// 死亡時に当たり判定を消す
			Collider2D col;
			if ((col = GetComponent<Collider2D>()) != null)
				col.enabled = false;
		}

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