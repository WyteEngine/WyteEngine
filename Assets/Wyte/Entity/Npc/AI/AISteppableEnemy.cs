using UnityEngine;

/// <summary>
/// 踏みつけて倒せるタイプの敵AI
/// </summary>
public class AISteppableEnemy : AIBaseBehaviour
{
	[SerializeField]
	private int attack;
	public int Attack
	{
		get { return attack; }
		set { attack = value; }
	}

	[SerializeField]
	[Tooltip("プレイヤーが踏みつけて倒すことをサポートするかどうか")]
	private bool playerInteractable = true;

	/// <summary>
	/// プレイヤーが踏みつけて倒すことをサポートするかどうか取得または設定します．
	/// </summary>
	public bool PlayerInteractable
	{
		get { return playerInteractable; }
		set { playerInteractable = value; }
	}

	protected override void OnInitialize()
	{
		OnCollidedWithPlayer = new SelectorNode(
			// 相手を殴る
			new SequenceNode(
				// プレイヤーが下降中でないか判定
				new IfNode(c => (Wyte.CurrentPlayer.Velocity.y >= 0 && !c.Dying) || !playerInteractable),
				// ダメージを与える
				new ActionNode(c => Wyte.CurrentPlayer.Damage(c, Attack))
			),
			// 相手に踏まれる><
			new SequenceNode(
				// 相手が下降中かどうか判定
				new IfNode(c => Wyte.CurrentPlayer.Velocity.y < 0 && playerInteractable),
				// ダメージを受ける
				new ActionNode(c => c.Damage(Wyte.CurrentPlayer, 1)),
				new ActionNode(c => Wyte.CurrentPlayer.Velocity += new Vector2(0, 128)),
				new ActionNode(c => Sfx.Play("entity.player.step"))
			)
		);
	}
}