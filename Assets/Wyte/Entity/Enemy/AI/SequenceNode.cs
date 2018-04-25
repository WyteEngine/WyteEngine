public class SequenceNode : AINodeBase
{
	private readonly AINodeBase[] nodes;

	public override bool Run()
	{
		var flag = true;
		foreach (var node in nodes)
		{
			if (!node.Run())
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