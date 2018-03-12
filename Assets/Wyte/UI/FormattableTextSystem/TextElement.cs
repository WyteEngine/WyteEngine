using System.Text;

public struct TextElement
{
	public string Text { get; set; }
	public float WaitTime { get; set; }
	public bool Bold { get; set; }
	public bool Italic { get; set; }
	public int Size { get; set; }
	public bool Nod { get; set; }
	public float Speed { get; set; }
	public string Color { get; set; }

	public override string ToString()
	{
		var sb = new StringBuilder().Append(Text);

		if (Bold)
			sb.Insert(0, "<b>").Append("</b>");
		if (Italic)
			sb.Insert(0, "<i>").Append("</i>");
		if (Size != default(int))
			sb.Insert(0, $"<size={Size}>").Append("</size>");
		if (!string.IsNullOrEmpty(Color))
			sb.Insert(0, $"<color={Color}>").Append("</color>");

		return sb.ToString();
	}
}