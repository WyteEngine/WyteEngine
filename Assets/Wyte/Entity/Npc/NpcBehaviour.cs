using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC なオブジェクトにアタッチします。
/// </summary>
public class NpcBehaviour : LivableEntity, IEventable
{

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
	[SerializeField]
	string killedAnimId;
	[SerializeField]
	string steppedAnimId;

	[Header("Sound FX Id")]
	[SerializeField]
	string landSfxId;
	[SerializeField]
	string jumpSfxId;
	[SerializeField]
	string deathSfxId;

	[Header("Entity Setting")]
	[SerializeField]
	float gravityScaleMultiplier = 1;

	public float GravityScaleMultiplier
	{
		get { return gravityScaleMultiplier; }
		set { gravityScaleMultiplier = value; Velocity = new Vector2(Velocity.x, 0); }
	}

	public bool IsManagedNpc { get; set; }

	public override string WalkAnimationId => walkAnimId;
	public override string StayAnimationId => stayAnimId;
	public override string JumpAnimationId => jumpAnimId;
	public override string LandSfxId => landSfxId;
	public override string JumpSfxId => jumpSfxId;
	public override string DeathSfxId => deathSfxId;

	public override float GravityScale => charaGravityScale * gravityScaleMultiplier;


	protected override void Start()
	{
		base.Start();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}


	protected override void CheckCollision(bool intersects)
	{
		if (intersects)
		{
			if (!string.IsNullOrWhiteSpace(label) && (EventKeyPushed && eventWhen == EventCondition.Talked) || !prevIntersects && eventWhen == EventCondition.Touched)
				Novel.Run(label);
		}

		base.CheckCollision(intersects);
	}

	protected override IEnumerator OnDeath(Object killer)
	{
		if (!string.IsNullOrWhiteSpace(label) && EventWhen == EventCondition.Dead)
			Novel.Run(label);
		else
		{
			if (killer == Wyte.CurrentPlayer)
			{
				// 踏まれた
				ChangeSprite(steppedAnimId);
				rigid.velocity = Velocity = Vector2.zero;
				yield return new WaitForSeconds(3);
			}
		}
		yield return new WaitWhile(() => Novel.Runtime.IsRunning);
	}

	public override void ChangeSprite(string id)
	{
		base.ChangeSprite(id);

		if (IsManagedNpc)
		{
			walkAnimId = stayAnimId = jumpAnimId = id;
		}
	}

	bool EventKeyPushed => IsSmartDevice ? GamePadBehaviour.Instance.Get(GamePadButtons.Screen, true) : Input.GetKeyDown(KeyBind.Up);

}
