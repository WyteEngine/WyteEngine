using System;

public class ActionNode : AINodeBase
{
	private readonly Action<Entity> action;

	public ActionNode(Action<Entity> act)
	{
		action = act;
	}

	public override bool Run(Entity context)
	{
		action(context);
		return true;
	}
}