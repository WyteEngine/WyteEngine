public class SelectorNode : AINodeBase
{
	private readonly AINodeBase[] nodes;

	public override bool Run(Entity context)
	{
		var flag = false;
		foreach (var node in nodes)
		{
			if (node.Run(context))
			{
				flag = true;
				break;
			}
		}
		return flag;
	}

	public SelectorNode(params AINodeBase[] nodeToSelect)
	{
		nodes = nodeToSelect;
	}
}