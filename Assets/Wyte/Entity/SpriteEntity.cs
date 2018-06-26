using System.Collections;
using UnityEngine;
/// <summary>
/// アニメーション可能なスプライトを持つEntityです．
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteEntity : Entity
{
	[SerializeField]
	private new WyteAnimation animation;
	public WyteAnimation Animation
	{
		get { return animation; }
		set { animation = value; }
	}

	/// <summary>
	/// アニメーションしているかどうかを取得します．
	/// </summary>
	/// <value>アニメーションしていればtrue，していなければfalse．</value>
	public bool IsAnimating { get; private set; }

	public WyteAnimation.AnimData CurrentAnim => Animation != null && animPtr < Animation.Count ? Animation[animPtr] : null;

	protected SpriteDirection direction;

	/// <summary>
	/// アニメーションの位置．
	/// </summary>
	int animPtr;
	/// <summary>
	/// アニメーション用に使う時間変数．
	/// </summary>
	float time;
	/// <summary>
	/// このEntityのSpriteRenderer
	/// </summary>
	protected SpriteRenderer spriteRenderer;

	Vector3 localScale;

	int loopTimes;

	public float AnimationMultiplier { get; protected set; } = 1;


	/// <summary>
	/// キャラの向き
	/// </summary>
	public enum SpriteDirection
	{
		Right,
		Left
	}

	public void StartAnim()
	{
		IsAnimating = true;
	}

	public void StopAnim()
	{
		IsAnimating = false;
	}

	public virtual void ChangeSprite(string id)
	{
		if (Animation == AnimMan[id] && IsAnimating)
			return;
		Animation = AnimMan[id];
		if (Animation != null)
			spriteRenderer.sprite = Animation[0].Sprite;
		animPtr = 0;
		loopTimes = 0;
		StartAnim();
	}

	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		localScale = transform.localScale;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnUpdate()
	{
		transform.localScale = new Vector3(direction == SpriteDirection.Left ? -localScale.x : localScale.x, localScale.y);

		if (AnimationMultiplier == 0f)
		{
			Debug.LogWarning("Animation Multiplier setting is now 0. It will be a cause of division by zero. Therefore, the system replaced it to 1.");
			AnimationMultiplier = 1;
		}

		// 無敵のときにチカチカする
		spriteRenderer.enabled = GodTime > 0 ? (GodTime * 1000 % 250 < 125) : true;
		spriteRenderer.enabled = GodTime <= 0 || (GodTime * 1000 % 250 < 125);

		if (Animation != null && IsAnimating)
		{
			spriteRenderer.sprite = CurrentAnim.Sprite;

			time += Time.deltaTime;
			if (time > CurrentAnim.Time * (1 / AnimationMultiplier))
			{
				animPtr++;
				time = 0;

				if (animPtr >= Animation.Count)
				{
					animPtr = 0;
					loopTimes++;
					if (Animation.UseLoop && Animation.LoopTimes != 0 && Animation.LoopTimes < loopTimes)
					{
						StopAnim();
						loopTimes = 0;
					}
				}
			}
		}
	}
}
