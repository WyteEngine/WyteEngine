using UnityEngine;

public class AIWalkAndWrapOnTheWall : AIBaseBehaviour
{
	[SerializeField]
	private bool startWalkingFromLeft;
	public bool StartWalkingFromLeft
	{
		get { return startWalkingFromLeft;}
		set { startWalkingFromLeft = value;}
	}
	

	[SerializeField]
	private int speed;
	public int Speed
	{
		get { return speed;}
		set { speed = value;}
	}
	
	protected override void OnInitialize()
	{
		LivableEntity le = null;
		OnUpdate = new SelectorNode(
			new SequenceNode(
				new IfNode(c => c is LivableEntity),
				new ActionNode(c => le = c as LivableEntity),
				new IfNode(c => le.CanKickRight()),
				new ActionNode(c => le.Move(-Speed))
			),
			new SequenceNode(
				new IfNode(c => c is LivableEntity),
				new ActionNode(c => le = c as LivableEntity),
				new IfNode(c => le.CanKickLeft()),
				new ActionNode(c => le.Move(Speed))
			),
			new SequenceNode(
				new IfNode(c => c is LivableEntity),
				new ActionNode(c => le = c as LivableEntity),
				new IfNode(c => le.Velocity.x == 0),
				new ActionNode(c => le.Move(StartWalkingFromLeft ? -Speed : Speed))
			)
		);
	}
}