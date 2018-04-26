public class IfNode : AINodeBase
{
	private readonly Condition condition;
	public IfNode(Condition c)
	{
		condition = c;
	}
	public override bool Run(Entity context) => condition(context);
}

public delegate bool Condition(Entity context);