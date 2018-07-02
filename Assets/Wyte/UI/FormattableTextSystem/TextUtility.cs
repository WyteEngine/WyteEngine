using System.Text.RegularExpressions;
namespace WyteEngine.UI.TextFormatting.Helper
{

	public static class TextUtility
	{
		public static string RemoveTags(string text) => Regex.Replace(text, @"<("".*? ""|'.*?'|[^'""])*?>", "");

		public static string RemoveFTSCommands(string text) => Regex.Replace(text, @"$[\w\d_]+?(?:\[.+\])?", "");

		public static string ToSafeString(string text) => RemoveFTSCommands(RemoveTags(text));
	}
}