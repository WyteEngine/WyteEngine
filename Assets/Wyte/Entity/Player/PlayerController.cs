using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// プレイヤーキャラの制御を行います．
/// </summary>
public class PlayerController : LivableEntity
{

	[Header("ANIMATION_NAME")]
	[SerializeField]
	string jumpAnimationId = "entity.player.jump";
	[SerializeField]
	string stayAnimationId = "entity.player.stay";
	[SerializeField]
	string walkAnimationId = "entity.player.walk";
	[SerializeField]
	protected float charaMoveSpeed = 5.0f;
	[SerializeField]
	protected float charaDashMultiplier = 2.0f;

	public override string JumpAnimationId => jumpAnimationId;
	public override string StayAnimationId => stayAnimationId;
	public override string WalkAnimationId => walkAnimationId;

	public override string LandSfxId => "entity.player.land";
	public override string JumpSfxId => "entity.player.jump";
	public override string DeathSfxId => "entity.player.death";

	public IEventable CurrentNpc => currentNpc;

	private IEventable currentNpc;


	/// <summary>
	/// 開始処理
	/// </summary>
	protected override void Start()
	{
		base.Start();
		Debugger.DebugRendering += Debugger_DebugRendering;
	}

	private void OnDestroy()
	{
		Debugger.DebugRendering -= Debugger_DebugRendering;
	}

	void Debugger_DebugRendering(System.Text.StringBuilder d)
	{

		d.Append($"pp{(int)transform.position.x},{(int)transform.position.y} ")
		 .Append($"pv{(int)rigid.velocity.x},{(int)rigid.velocity.y} ")
		 .Append($"p{(Dying ? "DEAD" : "ALIVE")} ");
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		// 移動可能時に処理を行う
		if (Wyte.CanMove)
			InputKey();
	}

	bool GetJumpKeyPushed(bool down = false) =>
		IsSmartDevice 
		// Android iOS など
		? GamePadBehaviour.Instance.Get(GamePadButtons.Action, down)
		// PC
		: (down ? Input.GetKeyDown(KeyBind.Jump) : Input.GetKey(KeyBind.Jump));

	bool EventKeyPushed =>
		IsSmartDevice ? GamePadBehaviour.Instance.Get(GamePadButtons.Screen, true) : Input.GetKeyDown(KeyBind.Up);

	/// <summary>
	/// キー入力
	/// </summary>
	void InputKey()
	{
		IsJumping &= (!IsGrounded() || (int)rigid.velocity.y != 0);
		if (GetJumpKeyPushed(true))
		{
			Jump();
		}
		if (EventKeyPushed && currentNpc != null)
		{
			Novel.Run(currentNpc.Label);
		}

		AnimationMultiplier = DashMultiplier;

		Move(KeyBind.Arrow.x * charaMoveSpeed * DashMultiplier, !GetJumpKeyPushed());
	}

	public float DashMultiplier =>
		Input.GetKey(KeyBind.Dash) ? charaDashMultiplier 
		: IsSmartDevice ? charaDashMultiplier
		: 1;

	/// <summary>
	/// 移動
	/// </summary>
	public void Move(float rightSpeed)
	{
		base.Move(rightSpeed);

		// 左端処理
		if (transform.position.x < Map.CurrentMapSize.xMin)
		{
			transform.position = new Vector3(Map.CurrentMapSize.xMin, transform.position.y, transform.position.z);
			rigid.velocity = new Vector2(rigid.velocity.x < 0 ? 0 : rigid.velocity.x, rigid.velocity.y);
		}

		// 右端処理
		if (transform.position.x > Map.CurrentMapSize.xMax)
		{
			transform.position = new Vector3(Map.CurrentMapSize.xMax, transform.position.y, transform.position.z);
			rigid.velocity = new Vector2(rigid.velocity.x > 0 ? 0 : rigid.velocity.x, rigid.velocity.y);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var npc = collision.GetComponent<IEventable>();
		if (npc != null)
		{
			currentNpc = npc;
			// 触れるだけで発動するイベントはここで処理
			if (currentNpc.EventWhen == EventCondition.Touched)
			{
				Novel.Run(currentNpc.Label);
				currentNpc = null;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (currentNpc == collision.GetComponent<IEventable>())
		{
			currentNpc = null;
		}
	}

	protected override IEnumerator OnDeath(Object killer)
	{
		StartCoroutine(Bgm.Stop(0.5f));
		Sfx.Play(DeathSfxId);
		ChangeSprite("entity.player.drown");
		for (int y = (int)transform.position.y; y < transform.position.y + 300; y += 4)
		{
			transform.Rotate(Vector3.forward * 4);
			transform.position = new Vector3(transform.position.x, y, transform.position.z);
		}
		yield return new WaitForSeconds(3);
		Wyte.Initalize();
	}
}
