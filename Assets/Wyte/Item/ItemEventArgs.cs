using System.Collections;

public class ItemEventArgs : System.EventArgs
{
	public IEnumerator Say(string text) => MessageContoller.Instance.Say(null, text);

	public string UserName { get; }
	public string ItemName { get; }
	public bool Canceled { get; set; }

	public ItemEventArgs(string user, string item, bool canceled = false)
	{
		UserName = user;
		ItemName = item;
		Canceled = canceled;
	}
}