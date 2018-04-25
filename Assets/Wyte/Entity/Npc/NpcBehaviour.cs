using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC なオブジェクトにアタッチします。
/// </summary>
public class NpcBehaviour : LivableEntity, IEventable {

	[Header("Event")]
	[SerializeField]
	[Tooltip("アクション時に実行するイベント ラベル。")]
	string label;
	public string Label
	{
		get { return label; }
		set { label = value; }
	}

	[SerializeField]
	[Tooltip("イベントの発火条件。")]
	EventCondition eventWhen = EventCondition.Talked;
	public EventCondition EventWhen => eventWhen;

	[Header("Animation Id")]
	[SerializeField]
	string stayAnimId;
	[SerializeField]
	string jumpAnimId;
	[SerializeField]
	string walkAnimId;

	[Header("Sound FX Id")]
	[SerializeField]
	string landSfxId;
	[SerializeField]
	string jumpSfxId;
	[SerializeField]
	string deathSfxId;

	[Header("Entity Setting")]
	[SerializeField]
	float gravityScale = 16;

	public override string WalkAnimationId => walkAnimId;
	public override string StayAnimationId => stayAnimId;
	public override string JumpAnimationId => jumpAnimId;
	public override string LandSfxId => landSfxId;
	public override string JumpSfxId => jumpSfxId;
	public override string DeathSfxId => deathSfxId;

	public override float GravityScale => gravityScale;

	protected new BoxCollider2D collider2D;
	protected BoxCollider2D playerCollider;

	/// <summary>
	/// 前フレームでのプレイヤー衝突判定．
	/// </summary>
	protected bool prevIntersects;

	protected override void Start()
	{
		base.Start();
		collider2D = GetComponent<BoxCollider2D>();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		var sp = CurrentAnim?.Sprite;
		if (sp != null)
		{
			charaWidth = charaWidth2 = sp.bounds.size.x;
			charaHead = sp.bounds.max.y;
			charaFoot = sp.bounds.min.y;
			collider2D.size = sp.bounds.size;
		}


		CheckCollision();
	}

	protected virtual void CheckCollision()
	{
		var intersects = IsCollidedWithPlayer();

		if (intersects)
		{
			if ((EventKeyPushed && eventWhen == EventCondition.Talked) || !prevIntersects && eventWhen == EventCondition.Touched)
				Novel.Run(label);
		}
		prevIntersects = intersects;
	}

	protected virtual bool IsCollidedWithPlayer()
	{
		if (playerCollider == null)
			playerCollider = Wyte.CurrentPlayer?.GetComponent<BoxCollider2D>();
		// プレイヤーが存在しなければ常にfalse
		if (playerCollider == null)
			return false;

		// 動けないのに死んだら理不尽だ
		if (!Wyte.CanMove)
			return false;
		
		return collider2D.bounds.Intersects(playerCollider.bounds);
		
	}

	protected override IEnumerator OnDeath(Object killer)
	{
		if (!string.IsNullOrWhiteSpace(label) && EventWhen == EventCondition.Dead)
			Novel.Run(label);
		
		yield return new WaitWhile(() => Novel.Runtime.IsRunning);
	}

	bool EventKeyPushed => IsSmartDevice ? GamePadBehaviour.Instance.Get(GamePadButtons.Screen, true) : Input.GetKeyDown(KeyBind.Up);

}
