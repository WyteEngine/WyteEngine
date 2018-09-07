using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Novel.Models;
using Novel.Parsing;
using System;
using Novel.Exceptions;
using WyteEngine.Event;
using WyteEngine.UI;

namespace WyteEngine.Event
{
	/// <summary>
	/// Novel スクリプトを用いてゲームを制御します。
	/// </summary>
	public class EventController : SingletonBaseBehaviour<EventController>
	{
		UnityNRuntime runtime;
		public UnityNRuntime Runtime => runtime;

		IEnumerable<TextAsset> Load() => Resources.LoadAll("Event", typeof(TextAsset)).Cast<TextAsset>();

		protected override void Awake()
		{
			base.Awake();

			// ランタイムの用意
			runtime = new UnityNRuntime(Load(), MessageContoller.Instance.Say);
		}

		// Use this for initialization
		void Start()
		{
			#region WyteEngine Novel API の登録
			// hack マネージャーの読み込みがまともになり次第ちゃんとする

			Runtime.Register("onplatform", OnPlatform);

			#endregion

		}

		public void Run(string label) => StartCoroutine(runtime.Call(label));
		// Update is called once per frame
		protected override void Update()
		{
			base.Update();
			// スクリプトリロード
			if (Wyte.IsDebugMode && Input.GetKeyDown(KeyCode.F2))
			{
				runtime.Reload(Load());
				Debug.Log("<color=yellow>スクリプトを再読込しました．</color>", this);
			}
		}


		bool PlatformIsMatch(string platformId)
		{
			foreach (var s in platformId.ToLower().Split(','))
			{
				switch (Application.platform)
				{
					case RuntimePlatform.WindowsEditor:
					case RuntimePlatform.WindowsPlayer:
						if (s == "windows" || s == "pc")
							return true;
						break;
					case RuntimePlatform.OSXEditor:
					case RuntimePlatform.OSXPlayer:
						if (s == "mac" || s == "pc")
							return true;
						break;
					case RuntimePlatform.LinuxEditor:
					case RuntimePlatform.LinuxPlayer:
						if (s == "linux" || s == "pc")
							return true;
						break;
					case RuntimePlatform.Android:
						if (s == "android" || s == "mobile")
							return true;
						break;
					case RuntimePlatform.IPhonePlayer:
						if (s == "ios" || s == "mobile")
							return true;
						break;
					case RuntimePlatform.WebGLPlayer:
						if (s == "web")
							return true;
						break;
					case RuntimePlatform.WSAPlayerARM:
					case RuntimePlatform.WSAPlayerX86:
					case RuntimePlatform.WSAPlayerX64:
						if (s == "uwp" || s == "mobile")
							return true;
						break;
				}
			}
			return false;
		}

		IEnumerator OnPlatform(string _, params string[] args)
		{
			NArgsAssert(args.Length % 3 == 0);
			string platform, gotogosub, label;
			platform = gotogosub = label = "";
			foreach (var a in args.Select((x, n) => new { x, n }))
			{
				switch (a.n % 3)
				{
					case 0:
						platform = a.x;
						break;
					case 1:
						NArgsAssert(a.x == "goto" || a.x == "gosub", a.n);
						gotogosub = a.x;
						break;
					case 2:
						label = a.x;
						if (PlatformIsMatch(platform))
							if (gotogosub == "goto")
								yield return Novel.Runtime.Goto(null, label);
							else
								yield return Novel.Runtime.Gosub(null, label);

						// 念の為
						platform = gotogosub = label = "";
						break;
				}
			}
		}


	}

	public delegate IEnumerator UnityNCommand(string spriteTags, params string[] args);

	/// <summary>
	/// Unity のコルーチンとして動作する Novel ランタイム．
	/// </summary>
	public class UnityNRuntime
	{
		/// <summary>
		/// 実行可能な Novel コード．
		/// </summary>
		INCode code;

		/// <summary>
		/// Novel コマンドのテーブル．
		/// </summary>
		Dictionary<string, UnityNCommand> commands;

		/// <summary>
		/// 現在実行されている Novel プログラムのアドレス．
		/// </summary>
		public int ProgramCounter { get; private set; }

		/// <summary>
		/// サブルーチン実行用のスタック
		/// </summary>
		private Stack<int> goSubStack;


		/// <summary>
		/// 実行中であるかどうか．
		/// </summary>
		/// <value><c>tsrue</c> if is running; otherwise, <c>false</c>.</value>
		public bool IsRunning { get; private set; }

		/// <summary>
		/// エラー出力用 Novel コマンド。
		/// </summary>
		UnityNCommand outErr;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UnityNRuntime"/> class.
		/// </summary>
		/// <param name="assets">スクリプト アセット。</param>
		/// <param name="errorCommand">エラー時の出力に使用するNovel コマンド。</param>
		public UnityNRuntime(IEnumerable<TextAsset> assets, UnityNCommand errorCommand)
		{
			commands = new Dictionary<string, UnityNCommand>();
			// combined text asset
			Reload(assets);
			outErr = errorCommand;
			goSubStack = new Stack<int>();

			#region 組み込みコマンド登録
			commands["goto"] = Goto;
			commands["debug"] = Debug;
			commands["wait"] = Wait;
			commands["end"] = End;
			commands["gosub"] = Gosub;
			commands["return"] = Return;
			#endregion
		}

		public void Reload(IEnumerable<TextAsset> assets)
		{
			// combined text asset
			var text = string.Join("\n", assets.Select(a => a.text));
			code = NParser.Parse(text);
		}


		/// <summary>
		/// Null チェックを行います
		/// 
		/// </summary>
		/// <param name="target">Target.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		void NullCheck<T>(T target) where T : class
		{
			if (target == null) throw new ArgumentNullException();
		}

		/// <summary>
		/// Novel コマンドの登録を行います．
		/// </summary>
		/// <returns>このインスタンスをそのまま返します．これによってメソッドチェーンが可能です．</returns>
		/// <param name="name">Name.</param>
		/// <param name="command">Command.</param>
		public UnityNRuntime Register(string name, UnityNCommand command)
		{
			// 申し訳ないが重複はNG.
			if (commands.ContainsKey(name))
				throw new ArgumentException($"The command name {name} is duplicated.");
			#region NullCheck
			NullCheck(name);
			NullCheck(command);
			// 通る場合，それは通常ならありえない挙動．クラスのバグか，リフレクション等の不正アクセス，またはランタイムのバグ．
			if (commands == null)
				throw new InvalidOperationException("bug: Command Dictionary is null");
			#endregion

			commands.Add(name, command);

			return this;
		}

		/// <summary>
		/// 指定されたラベルから，コード実行を行います．ラベルが空であったり，存在しない場合冒頭から実行します．
		/// </summary>
		/// <returns>コルーチンを返却します．MonoBehaviourで実行するようにしてください．</returns>
		/// <param name="label">Label.</param>
		public IEnumerator Call(string label = default(string))
		{
			ProgramCounter = 0;
			NullCheck(code);
			label = NovelHelper.GetLabelString(label);
			if (!string.IsNullOrWhiteSpace(label))
				if (code.Labels.ContainsKey(label))
					ProgramCounter = code.Labels[label];
			IsRunning = true;
			return StartEngine();
		}

		/// <summary>
		/// ランタイムのコルーチンです．
		/// </summary>
		/// <returns>The engine.</returns>
		IEnumerator StartEngine()
		{
			GameMaster.Instance.CanMove = false;
			GameMaster.Instance.IsNotFreezed = false;

			while (true)
			{
				// コードの終端に到達したら終了．
				if (ProgramCounter > code.Statements.Length - 1)
					break;
				// stop
				if (!IsRunning)
					break;
				// ステートメントを取得
				var statement = code.Statements[ProgramCounter];
				if (!commands.ContainsKey(statement.CommandName))
				{
					UnityEngine.Debug.LogWarning($"Command Not Found `{statement.CommandName}`");
				}
				else
				{
					IEnumerator command = commands[statement.CommandName.ToLower()](statement.SpriteTag, statement.Arguments);

					bool flag = true;;
					while (true)
					{
						try
						{
							if (!(flag = command.MoveNext()))
								break;
						}
						catch (NRuntimeException ex)
						{
							// 意図的なエラー．
							UnityEngine.Debug.LogError($"Error in novel script(line.{ProgramCounter})\n{ex.Message}");
							continue;
						}
						catch (Exception ex)
						{
							// なんかのバグ．
							UnityEngine.Debug.LogError($"Unhandled Exception in novel script(line.{ProgramCounter}) due to a bug.\n{ex.Message}\n{ex.StackTrace}");
							continue;
						}
						if (command.Current != null)
						{
							yield return command.Current;
						}
					}
				}

				ProgramCounter++;

			}
			GameMaster.Instance.CanMove = true;
			GameMaster.Instance.IsNotFreezed = true;
			IsRunning = false;
		}

		#region 組み込みコマンド用ヘルパーメソッド

		int GetLine(string label)
		{
			// Nullチェックと整形
			label = NovelHelper.GetLabelString(label);

			if (!code.Labels.ContainsKey(label))
				throw new NRuntimeException($"ラベル \"{label}\"が存在しません．");
			return code.Labels[label];
		}

		#endregion

		public void Goto(int ptr) => ProgramCounter = ptr - 1;

		#region 組み込みコマンド実装
		public IEnumerator Goto(string t, params string[] args)
		{
			if (args.Length == 0)
				throw new NRuntimeException("移動先のラベルが指定されていません．");
			var label = GetLine(NovelHelper.CombineAll(args));
			// 移動する
			Goto(label);
			yield break;
		}

		public IEnumerator Gosub(string _, params string[] args)
		{
			if (goSubStack.Count > 20)
				throw new NRuntimeException("スタックオーバーフローです．");
			goSubStack.Push(ProgramCounter + 1);
			return Goto(_, args);
		}

		public IEnumerator Return(string _, params string[] args)
		{
			if (goSubStack.Count < 1)
				throw new NRuntimeException("サブルーチンにいません．");
			Goto(goSubStack.Pop());
			yield break;
		}

		public IEnumerator Debug(string t, params string[] args)
		{
			UnityEngine.Debug.Log($"{t}+Debug {string.Join(", ", args)}");
			yield break;
		}


		public IEnumerator Wait(string spriteTags, string[] args)
		{
			float i;
			if (!float.TryParse(NovelHelper.CombineAll(args), out i))
				throw new NRuntimeException("不正な数値です．");
			yield return new WaitForSeconds(i);
		}

		public IEnumerator End(string tag, string[] args)
		{
			IsRunning = false;
			yield break;
		}
		#endregion

	}

	public static class NovelHelper
	{
		public static float TryParse(string numeric)
		{
			float ret;
			if (!float.TryParse(numeric, out ret))
				throw new NRuntimeException("型が一致しません．");
			return ret;
		}

		/// <summary>
		/// 問答無用で文字列配列をそのまま連結します．引数の区切りがいらない場合に便利です．
		/// </summary>
		/// <returns>連結された文字列．</returns>
		/// <param name="strings">連結してほしい文字列を含む配列．</param>
		public static string CombineAll(string[] strings) => string.Join("", strings);

		/// <summary>
		/// ラベル文字列を整形します．
		/// </summary>
		/// <returns>The label string.</returns>
		/// <param name="label">Label.</param>
		public static string GetLabelString(string label)
		{
			// null or empty チェック
			if (string.IsNullOrEmpty(label))
				throw new ArgumentNullException(nameof(label));

			// 仕様上，#をつけてもつけなくてもよい
			if (label[0] == '#')
				label = label.Remove(0, 1);
			return label;
		}
	}
}