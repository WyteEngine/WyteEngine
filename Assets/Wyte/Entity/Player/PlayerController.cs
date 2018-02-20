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

		// しないと CanMove でないときにｽｨｰってなる
		rigid.velocity = new Vector2(0, rigid.velocity.y);

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
	public override void Move(float rightSpeed, bool hold = true)
	{
		base.Move(rightSpeed, hold);

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

	protected override IEnumerator OnDeath(Object killer)
	{
		StartCoroutine(Bgm.Stop(0.5f));
		Sfx.Play(DeathSfxId);
		ChangeSprite("entity.player.drown");
		var targetY = transform.position.y + 300;
		for (int y = (int)transform.position.y; y < targetY; y += 4)
		{
			transform.Rotate(Vector3.forward * 180 * Time.deltaTime);
			transform.position = new Vector3(transform.position.x, y, transform.position.z);
			yield return null;
		}
		yield return new WaitForSeconds(3);
		Wyte.Initalize();
	}
}
