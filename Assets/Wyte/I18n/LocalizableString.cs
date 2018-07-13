namespace WyteEngine.I18n
{
	/// <summary>
	/// 文字列リテラル感覚でローカライズをサポートするためのクラス．
	/// </summary>
	public class LocalizableString
	{
		public string Key { get; }

		public LocalizableString(string key)
		{
			Key = key;
		}

		public static implicit operator LocalizableString(string key) => new LocalizableString(key);

		public static implicit operator string(LocalizableString lstr) => I18nProvider.Instance[lstr.Key];

		/// <summary>
		/// 翻訳済み文字列を返します．
		/// </summary>
		public override string ToString() => this;
	}

}