using System.Collections;
using UnityEngine;

/// <summary>
/// 重力の影響を受け，移動可能な Entity です．
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class LivableEntity : SpriteEntity
{
	public virtual float GravityScale => charaGravityScale;

	public abstract string WalkAnimationId { get; }
	public abstract string StayAnimationId { get; }
	public abstract string JumpAnimationId { get; }

	public abstract string LandSfxId { get; }
	public abstract string JumpSfxId { get; }
	public abstract string DeathSfxId { get; }

	public bool IsJumping { get; protected set; }

	protected Rigidbody2D rigid;

	[SerializeField]
	protected LayerMask groundLayer;

	[Header("CHARACTER_STATUS")]
	[SerializeField]
	protected float charaScale = 1.0f;
	[SerializeField]
	protected float charaHead = 1.0f;
	[SerializeField]
	protected float charaFoot = -1.0f;
	[SerializeField]
	protected float charaWidth = 1.0f;
	[SerializeField]
	protected float charaWidth2 = 1.0f;
	[SerializeField]
	protected float charaGravityScale = 1.0f;
	[SerializeField]
	protected float charaJumpScale = 10.0f;
	[SerializeField]
	protected float charaCeilingBouness = -1.0f;

	protected bool prevIsGrounded, prevIsCeiling;

	protected override void Start()
	{
		base.Start();

		rigid = gameObject.GetComponent<Rigidbody2D>();
		rigid.freezeRotation = true;
		rigid.gravityScale = GravityScale;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		Animate();

		// 落下死
		if (Map.CurrentMap != null && transform.position.y < Map.CurrentMapSize.yMin)
			Kill(Map.CurrentMap);

		prevIsCeiling = IsCeiling();
		prevIsGrounded = IsGrounded();
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		// しないと CanMove でないときにｽｨｰってなる
		rigid.velocity = new Vector2(0, rigid.velocity.y);
	}

	/// <summary>
	/// アニメーション制御
	/// </summary>
	void Animate()
	{
		if (IsJumping)
			ChangeSprite(JumpAnimationId);
		else if ((int)Mathf.Round(rigid.velocity.x) == 0)
			ChangeSprite(StayAnimationId);
		else
			ChangeSprite(WalkAnimationId);
	}

	public virtual void Move(float rightSpeed, bool hold = true)
	{
		rigid.velocity = new Vector2(rightSpeed, rigid.velocity.y);
		direction = (int)rightSpeed < 0 ? SpriteDirection.Left : (int)rightSpeed > 0 ? SpriteDirection.Right : direction;
		// 着地音
		if ((IsCeiling() && !prevIsCeiling) || (IsGrounded() && !prevIsGrounded))
			Sfx.Play(LandSfxId);

		if (IsJumping && hold)
			rigid.velocity -= new Vector2(0, charaGravityScale * .4f);

		if (IsCeiling())
			rigid.velocity = new Vector2(rigid.velocity.x, charaCeilingBouness);
	}

	/// <summary>
	/// ジャンプした時
	/// </summary>
	public virtual void Jump()
	{
		if (IsGrounded())
		{
			var nowVec = rigid.velocity;
			rigid.velocity = new Vector3(nowVec.x, charaJumpScale);
			IsJumping = true;
			Sfx.Play(JumpSfxId);
		}

		/*else if (CanKickLeft())
		{
			rigid
		}
		else if (CanKickRight())
		{
			rigid.AddForce((Vector2.left + Vector2.up) * 3000);
		}*/
		//TODO: プレイヤーの左右移動に加速度を取り入れ、外部からの圧力を無視しないようにする
	}

	/// <summary>
	/// 地面についているかどうか
	/// </summary>
	/// <returns></returns>
	public bool IsGrounded()
	{
		var hit = Physics2D.Linecast(FloorA, FloorB, groundLayer);
		return hit;
	}

	/// <summary>
	/// 頭が当たったかどうか
	/// </summary>
	/// <returns></returns>
	public bool IsCeiling()
	{
		bool hit = Physics2D.Linecast(CeilingA, CeilingB, groundLayer);
		return hit;
	}

	Vector3 FloorA => transform.position + new Vector3(-(charaWidth / 2), charaFoot);
	Vector3 FloorB => transform.position + new Vector3((charaWidth / 2), charaFoot);

	Vector3 CeilingA => transform.position + new Vector3(-(charaWidth / 2), charaHead);
	Vector3 CeilingB => transform.position + new Vector3((charaWidth / 2), charaHead);

	public bool CanKickLeft() => Physics2D.Linecast(KickLA, KickLB, groundLayer);
	public bool CanKickRight() => Physics2D.Linecast(KickRA, KickRB, groundLayer);

	Vector3 KickLA => transform.position + new Vector3(-(charaWidth2 / 2), charaHead / 2);
	Vector3 KickLB => transform.position + new Vector3(-(charaWidth2 / 2), charaFoot / 2);

	Vector3 KickRA => transform.position + new Vector3((charaWidth2 / 2), charaHead / 2);
	Vector3 KickRB => transform.position + new Vector3((charaWidth2 / 2), charaFoot / 2);

}