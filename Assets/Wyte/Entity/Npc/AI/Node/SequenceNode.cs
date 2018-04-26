public class SequenceNode : AINodeBase
{
	private readonly AINodeBase[] nodes;

	public override bool Run(Entity context)
	{
		var flag = true;
		foreach (var node in nodes)
		{
			if (!node.Run(context))
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public SequenceNode(params AINodeBase[] nodeToSelect)
	{
		nodes = nodeToSelect;
	}
}