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
	public string Label => label;

	[SerializeField]
	[Tooltip("イベントの発火条件。")]
	EventCondition eventWhen;
	public EventCondition EventWhen => eventWhen;

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
	float gravityScale;

	public override string WalkAnimationId => walkAnimId;
	public override string StayAnimationId => stayAnimId;
	public override string JumpAnimationId => jumpAnimId;
	public override string LandSfxId => landSfxId;
	public override string JumpSfxId => jumpSfxId;
	public override string DeathSfxId => deathSfxId;

	public override float GravityScale => gravityScale;

	protected override IEnumerator OnDeath(Object killer)
	{
		if (!string.IsNullOrWhiteSpace(label))
			Novel.Run(label);
		
		yield return new WaitWhile(() => Novel.Runtime.IsRunning);
	}
}
