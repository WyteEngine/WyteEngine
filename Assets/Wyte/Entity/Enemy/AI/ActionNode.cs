using System;

public class ActionNode : AINodeBase
{
	private readonly Action action;

	public ActionNode(Action act)
	{
		action = act;
	}

	public override bool Run()
	{
		action();
		return true;
	}
}