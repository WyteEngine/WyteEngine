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
		get { return attack;}
		set { attack = value;}
	}
	

	protected override void OnInitialize()
	{
		OnCollidedWithPlayer = new SelectorNode(
			// 相手を殴る
			new SequenceNode(
				// プレイヤーが下降中でないか判定
				new IfNode(c => Wyte.CurrentPlayer.Velocity.y >= 0 && !c.Dying),
				// ダメージを与える
				new ActionNode(c => Wyte.CurrentPlayer.Damage(c, Attack))
			),
			// 相手に踏まれる><
			new SequenceNode(
				// 相手が下降中かどうか判定
				new IfNode(c => Wyte.CurrentPlayer.Velocity.y < 0),
				// ダメージを受ける
				new ActionNode(c => c.Damage(Wyte.CurrentPlayer, 1)),
				new ActionNode(c => Wyte.CurrentPlayer.Velocity += new Vector2(0, 128)),
				new ActionNode(c => Sfx.Play("entity.player.step"))
			)
		);
	}
}