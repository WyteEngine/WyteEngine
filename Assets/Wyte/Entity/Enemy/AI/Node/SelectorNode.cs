public class SelectorNode : AINodeBase
{
	private readonly AINodeBase[] nodes;

	public override bool Run()
	{
		var flag = false;
		foreach (var node in nodes)
		{
			if (node.Run())
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