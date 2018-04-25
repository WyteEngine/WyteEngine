public class IfNode : AINodeBase
{
	private readonly Condition condition;
	public IfNode(Condition c)
	{
		condition = c;
	}
	public override bool Run() => condition();
}

public delegate bool Condition();