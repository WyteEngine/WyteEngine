using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WyteEngine.I18n
{

	/// <summary>
	/// 国際化のための機能を提供します．
	/// </summary>
	public class I18nProvider : SingletonBaseBehaviour<I18nProvider>
	{
		[SerializeField]
		private string lang = "en-us";

		public string Language
		{
			get { return lang; }
			set
			{
				var oldLang = lang;
				lang = value;
				LanguageChanged?.Invoke(oldLang, lang);
			}
		}

		public Dictionary<string, Dictionary<string, string>> Locales { get; private set; }

		public Dictionary<string, string> CurrentLang => Locales.ContainsKey(Language) ? Locales[Language] : null;

		public event LanguageChangedEventHandler LanguageChanged;

		public string[] AvailableLocales { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			AvailableLocales = Resources.Load<TextAsset>("Language/langs").text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

			Locales = new Dictionary<string, Dictionary<string, string>>();
			foreach (var locale in AvailableLocales)
			{
				var asset = Resources.Load<TextAsset>($"Language/{locale}/strings");
				Locales[locale] = ParseLangFile(asset.text);
				Debug.Log($"Loaded a {locale} locale file.");
			}
		}

		public T GetResource<T>(string path) where T : Object
		{
			return Resources.Load<T>($"Language/{Language}/{path}");
		}

		/// <summary>
		/// 翻訳された文字列を取得します．
		/// </summary>
		/// <param name="key">Key.</param>
		public string this[string key] => CurrentLang != null ? (CurrentLang.ContainsKey(key) ? CurrentLang[key] : key) : key;

		/// <summary>
		/// 言語ファイルのパースを行います．
		/// </summary>
		private Dictionary<string, string> ParseLangFile(string text)
		{
			var dict = new Dictionary<string, string>();

			// 改行時のキー保存場所
			var keyTemp = "";

			// 改行処理フラグ
			var brFlag = false;
			foreach (var kv in ToLFString(text).Split('\n'))
			{
				// 空行を飛ばす
				if (string.IsNullOrWhiteSpace(kv))
					continue;

				if (brFlag)
				{
					dict[keyTemp] += '\n';
					var v = kv;
					brFlag = false;
					// ¥ か \ が含まれていれば改行フラグ
					if (EndWithEscape(v))
					{
						v = v.Remove(kv.Length - 1);
						brFlag = true;
					}
					dict[keyTemp] += v;
				}

				// 構文がおかしいやつは飛ばす
				if (!kv.Contains('='))
					continue;

				var split = kv.Split('=');

				// 構文がおかしいやつは飛ばす
				if (split.Length < 2)
					continue;

				var key = split[0].Trim();
				var value = string.Join("=", split.Skip(1)).Trim();

				// ¥ か \ が含まれていれば改行フラグ
				if (EndWithEscape(value))
				{
					value = value.Remove(value.Length - 1);
					brFlag = true;
					keyTemp = key;
				}
				// 辞書にぶちこむ(2つ目の:を考慮する)
				dict[key] = value;

			}

			return dict;
		}

		static bool EndWithEscape(string str) => str.Length > 0 && "\\¥".Contains(str.Last());

		/// <summary>
		/// 改行コードを統一した文字列に変換します．
		/// </summary>
		/// <returns>改行コードをLine Feedに統一した文字列．</returns>
		/// <param name="str">変換すべき文字列．</param>
		static string ToLFString(string str) => str.Replace("\r\n", "\n").Replace('\r', '\n');

		public delegate void LanguageChangedEventHandler(string oldLanguage, string newLanguage);
	}

}