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

	public Vector2 Velocity { get; set; }
	
	protected Rigidbody2D rigid;

	[SerializeField]
	protected LayerMask groundLayer;

	protected float charaScale = 1.0f;
	protected float charaHead = 1.0f;
	protected float charaFoot = -1.0f;
	protected float charaWidth = 1.0f;
	protected float charaWidth2 = 1.0f;
	protected float charaGravityScale = 148.0f;
	protected float charaJumpScale = 128.0f;
	protected float charaCeilingBouness = -1.0f;

	protected bool prevIsGrounded, prevIsCeiling;
	
	protected new BoxCollider2D collider2D;
	
	protected override void Start()
	{
		base.Start();

		rigid = gameObject.GetComponent<Rigidbody2D>();
		collider2D = GetComponent<BoxCollider2D>();
		rigid.freezeRotation = true;
		rigid.gravityScale = 0;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		Animate();

		var sp = CurrentAnim?.Sprite;
		if (sp != null)
		{
			charaWidth = sp.bounds.size.x * 0.625f;
			charaWidth2 = sp.bounds.size.x * 1.2f;
			charaHead = (sp.bounds.size.y / 2) * 1.2857143f;
			charaFoot = (sp.bounds.size.y / 2) * -1.25f;
			collider2D.size = sp.bounds.size;
		}

		// 落下死
		if (Map.CurrentMap != null && transform.position.y < Map.CurrentMapSize.yMin)
			Kill(Map.CurrentMap);

		prevIsCeiling = IsCeiling();
		prevIsGrounded = IsGrounded();
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		
		if (Dying)
		{
			Velocity.Set(0, 0);
		}
		
		if (Velocity.y < 0 && IsGrounded())
		{
			Velocity = new Vector2(Velocity.x, 0);
		}
		if (!IsGrounded())
		{
			Velocity -= new Vector2(0, GravityScale * Time.fixedDeltaTime);
		}
		// todo あとでもっとマシに
		rigid.velocity = Velocity;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = CanKickLeft() ? Color.red : Color.blue;
		Gizmos.DrawLine(KickLA, KickLB);
		
		Gizmos.color = CanKickRight() ? Color.red : Color.blue;
		Gizmos.DrawLine(KickRA, KickRB);
	}

	/// <summary>
	/// アニメーション制御
	/// </summary>
	void Animate()
	{
		if (IsJumping)
			ChangeSprite(JumpAnimationId);
		else if ((int)Mathf.Round(Velocity.x) == 0)
			ChangeSprite(StayAnimationId);
		else
			ChangeSprite(WalkAnimationId);
	}

	public virtual void Move(float rightSpeed, bool hold = true)
	{
		Velocity = new Vector2(rightSpeed, Velocity.y);
		direction = (int)rightSpeed < 0 ? SpriteDirection.Left : (int)rightSpeed > 0 ? SpriteDirection.Right : direction;
		// 着地音
		if ((IsCeiling() && !prevIsCeiling) || (IsGrounded() && !prevIsGrounded))
			Sfx.Play(LandSfxId);

		if (IsJumping && hold)
			Velocity -= new Vector2(0, GravityScale * 0.05f);

		if (IsCeiling())
			Velocity = new Vector2(Velocity.x, charaCeilingBouness);
	}

	/// <summary>
	/// ジャンプした時
	/// </summary>
	public virtual void Jump()
	{
		if (IsGrounded())
		{
			var nowVec = Velocity;
			Velocity = new Vector3(nowVec.x, charaJumpScale);
			IsJumping = true;
			Sfx.Play(JumpSfxId);
		}

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