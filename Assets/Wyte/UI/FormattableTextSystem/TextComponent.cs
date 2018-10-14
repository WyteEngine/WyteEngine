using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using WyteEngine.Map;
using WyteEngine.Entities;
using WyteEngine.Music;
using UnityEngine;
using WyteEngine.Inputing;

namespace WyteEngine.UI.TextFormatting
{

	public class TextComponent
	{
		public TextElement[] Elements { get; }

		public TextComponent(string text)
		{
			Elements = ParseCommand(text).ToArray();
		}

		public TextComponent(IEnumerable<TextElement> elements)
		{
			Elements = elements.ToArray();
		}

		public static string Parse(string text)
		{
			return ElementToText(ParseCommand(text).ToArray());
		}

		public override string ToString() => ElementToText(Elements);


		static List<TextElement> ParseCommand(string text)
		{
			var elems = new List<TextElement>();
			var element = new TextElement
			{
				Speed = 1
			};
			var state = ParseState.PlainText;
			var cmdName = new StringBuilder();
			var cmdArgs = new StringBuilder();

			for (int i = 0; i < text.Length; i++)
			{
				var c = text[i];
				var nc = i < text.Length - 1 ? text[i + 1] : default(char);
				switch (state)
				{
					case ParseState.PlainText:
						// コマンドはじまりか？
						if (c == '$')
						{
							// エスケープか？
							if (nc == '$')
							{
								// そのまま出力して1個飛ばす
								i++;
							}
							else
							{
								// コマンドをはじめる
								state = ParseState.Name;
								cmdName.Clear();
								cmdArgs.Clear();
								continue;
							}
						}
						element.Text = c.ToString();

						elems.Add(element);
						element.Nod = false;
						element.WaitTime = 0;
						break;
					case ParseState.Name:
						if (c == '=')
						{
							// 引数開始
							state = ParseState.Argument;
							continue;
						}

						if (c == ';')
						{
							// コマンド終了
							state = ParseState.PlainText;
							DoCommand(ref elems, ref element, cmdName.ToString(), cmdArgs.ToString());
							continue;
						}

						cmdName.Append(c);
						break;
					case ParseState.Argument:
						if (c == ';')
						{
							// コマンド終了
							state = ParseState.PlainText;
							DoCommand(ref elems, ref element, cmdName.ToString(), cmdArgs.ToString());
							continue;
						}

						cmdArgs.Append(c);
						break;
					default:
						throw new InvalidOperationException("Invalid parse state");
				}
			}

			return elems;
		}

		static void DoCommand(ref List<TextElement> list, ref TextElement elem, string name, string args)
		{
			switch (name.ToLower())
			{
				case "b":
				case "bold":
					elem.Bold = true;
					break;
				case "i":
				case "italic":
					elem.Italic = true;
					break;
				case "c":
				case "col":
				case "color":
					elem.Color = args;
					break;
				case "v":
				case "voice":
					elem.Voice = args;
					break;
				case "!b":
				case "!bold":
					elem.Bold = false;
					break;
				case "!i":
				case "!italic":
					elem.Italic = false;
					break;
				case "!c":
				case "!col":
				case "!color":
					elem.Color = null;
					break;
				case "!sz":
				case "!size":
					elem.Size = 0;
					break;
				case "!spd":
				case "!speed":
					elem.Speed = 1;
					break;
				case "sz":
				case "size":
					int size;
					if (!int.TryParse(args.Trim(), out size))
						throw new FormatException("サイズには整数値のみ指定できます");

					elem.Size = size;
					break;
				case "r":
				case "rest":
					elem.Bold = false;
					elem.Italic = false;
					elem.Color = null;
					elem.Nod = false;
					elem.Speed = 1;
					elem.WaitTime = 0;
					elem.Size = 0;
					elem.Voice = null;
					break;
				case "w":
				case "wait":
					float wait;
					if (!float.TryParse(args.Trim(), out wait))
						throw new FormatException("待機時間には実数値のみ指定できます");

					elem.WaitTime = wait;
					break;
				case "nod":
					elem.Nod = true;
					break;
				case "spd":
				case "speed":
					float spd;
					if (!float.TryParse(args.Trim(), out spd))
						throw new FormatException("文字送り速度には実数値のみ指定できます");

					elem.Speed = spd;
					break;
				case "var":
				case "variable":
					foreach (var c in GetVariable(args))
					{
						elem.Text = c.ToString();
						list.Add(elem);
						elem.Nod = false;
						elem.WaitTime = 0;
					}
					break;
				default:
					var raw = new StringBuilder().Append('$').Append(name);
					if (!string.IsNullOrEmpty(args))
						raw.Append('=').Append(args);
					raw.Append(';');
					foreach (var c in raw.ToString())
					{
						elem.Text = c.ToString();
						list.Add(elem);
						elem.Nod = false;
						elem.WaitTime = 0;
					}
					break;
			}
		}

		static string GetVariable(string key)
		{
			var p = GameMaster.Instance?.CurrentPlayer != null ? GameMaster.Instance.CurrentPlayer.GetComponent<PlayerController>() : null;


			var keys = KeyBinding.Instance.Binding;
			switch (key)
			{
				case "pname":
					return GameMaster.Instance.Player?.Name ?? "No Name";
				case "plife":
					return (p?.Health ?? 0).ToString();
				case "pmaxlife":
					return (p?.MaxHealth ?? 0).ToString();
				case "ppos":
					return p?.transform.position.ToString() ?? "";
				case "px":
					return (p?.transform.position.x ?? 0).ToString();
				case "py":
					return (p?.transform.position.y ?? 0).ToString();
				case "time":
					return DateTime.Now.ToString("HH:mm:ss");
				case "time_h":
					return DateTime.Now.Hour.ToString();
				case "time_m":
					return DateTime.Now.Minute.ToString();
				case "time_ms":
					return DateTime.Now.Millisecond.ToString();
				case "date":
					return DateTime.Now.ToString("yy/MM/dd");
				case "date_y":
					return DateTime.Now.Year.ToString();
				case "date_m":
					return DateTime.Now.Month.ToString();
				case "date_d":
					return DateTime.Now.Day.ToString();
				case "date_wd":
					return DateTime.Now.ToString("dddd");
				case "key_left":
					return ConvertKeyText(keys.Left);
				case "key_right":
					return ConvertKeyText(keys.Right);
				case "key_up":
					return ConvertKeyText(keys.Up);
				case "key_down":
					return ConvertKeyText(keys.Down);
				case "key_jump":
					return ConvertKeyText(keys.Jump);
				case "key_dash":
					return ConvertKeyText(keys.Dash);
				case "key_action":
					return ConvertKeyText(keys.Action);
				case "key_pause":
					return ConvertKeyText(keys.Pause);
				case "now":
					return DateTime.Now.ToString("yy/MM/dd HH:mm:ss");
				case "map_id":
				case "map_name":
					return MapManager.Instance?.CurrentMap != null ? MapManager.Instance.CurrentMap.name : "No Map";
				case "bgm_id":
				case "bgm_name":
					return MusicManager.Instance?.SongName ?? "None";
				default:
					return "";
			}
		}

		static string ElementToText(IEnumerable<TextElement> elements)
		{
			return string.Concat(elements.Select(e => e.ToString()));
		}

		static string ConvertKeyText(string text)
		{
			switch (text)
			{
				default:
					return text;
				case "up":
					return "↑";
				case "down":
					return "↓";
				case "left":
					return "←";
				case "right":
					return "→";
			}
		}


		enum ParseState
		{
			PlainText, Name, Argument
		}
	}

	/// <summary>
	/// <see cref="TextComponent"/> を生成するためのビルダークラスです。
	/// </summary>
	public class TextComponentBuilder
	{
		private readonly StringBuilder b = new StringBuilder();

		public string CurrentText => b.ToString();

		public static TextComponentBuilder Create() => new TextComponentBuilder();

		public TextComponentBuilder Text(string text) => Append(text);
		public TextComponentBuilder Bold(bool on = true) => Append(on ? "$b;" : "$!b;");
		public TextComponentBuilder Italic(bool on = true) => Append(on ? "$i;" : "$!i;");
		public TextComponentBuilder Color(Color c) => Append($"$c=#{ColorUtility.ToHtmlStringRGBA(c)};");
		public TextComponentBuilder Color(string c) => Append($"$c={c};");
		public TextComponentBuilder Size(int size) => Append($"$sz={size};");
		public TextComponentBuilder Reset() => Append("$r;");
		public TextComponentBuilder Wait(float time) => Append($"$w={time};");
		public TextComponentBuilder Nod() => Append("$nod;");
		public TextComponentBuilder Speed(float time) => Append($"$spd={time};");
		public TextComponentBuilder Variable(string var) => Append($"$var={var};");
		public TextComponentBuilder Voice(string v) => Append($"v={v};");

		public TextComponent Finish() => new TextComponent(CurrentText);

		private TextComponentBuilder Append(string text)
		{
			b.Append(text);
			return this;
		}
	}
}