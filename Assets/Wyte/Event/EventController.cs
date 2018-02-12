using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Novel.Models;
using Novel.Parsing;
using Novel;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Novel.Exceptions;

/// <summary>
/// Novel スクリプトを用いてゲームを制御します。
/// </summary>
public class EventController : SingletonBaseBehaviour<EventController>
{
	UnityNRuntime runtime;
	public UnityNRuntime Runtime => runtime;

	// Use this for initialization
	void Start()
	{
		// 裏でリソースをロードする
		var assets = Resources.LoadAll("Event", typeof(TextAsset));
		// ランタイムの用意
		runtime = new UnityNRuntime(assets.Cast<TextAsset>(), MessageContoller.Instance.Say);

		#region WyteEngine Novel API の登録

		runtime
			// Fade
			.Register("fade", FadeController.Instance.Fade)
			// Fade Asynchronously
			.Register("fadeasync", FadeController.Instance.FadeAsync)
			// Play BGM
			.Register("bgmplay", Bgm.Play)
			// Stop BGM
			.Register("bgmstop", Bgm.Stop)
			// Stop BGM Asynchronously
			.Register("bgmstopasync", Bgm.StopAsync)
			// Sound FX 
			.Register("sfx", Sfx.Play)
			// Alias
			.Register("se", Sfx.Play)
			// Map Moving
			.Register("move", Map.Move)
			 // Show Player
			.Register("pshow", Wyte.PlayerShow)
			 // Hide Player
			.Register("phide", Wyte.PlayerHide)
			// Edit Flag
			.Register("flag", Flag.Flag)
			// Event by Flag
			.Register("onflag", Flag.OnFlag)
			// Edit Skip Flag
			.Register("sflag", Flag.SkipFlag)
			// Event by Skip Flag
			.Register("onsflag", Flag.OnSkipFlag)
			// Say message
			.Register("say", MessageContoller.Instance.Say)
			// Freeze All
			.Register("freeze", Wyte.Freeze)
			// Freeze the Player
			.Register("pfreeze", Wyte.PlayerFreeze)
			// Switch to the PlayerCamera
			.Register("playercamera", Camera.SwitchToPlayerCamera)
			// Switch to the FreeCamera
			.Register("freecamera", Camera.SwitchToFreeCamera);


		#endregion

	}

	public void Run(string label) => StartCoroutine(runtime.Call(label));
	// Update is called once per frame
	void Update()
	{

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
		var text = string.Join("\n", assets.Select(a => a.text));
		code = NParser.Parse(text);
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
		label = GetLabelString(label);
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
				IEnumerator command = null;
				// 試行
				try
				{
					command = commands[statement.CommandName.ToLower()](statement.SpriteTag, statement.Arguments);
				}
				// スクリプトに起因するエラー。
				catch (NRuntimeException ex)
				{
					command = outErr?.Invoke("エラー:" + ex.Message);
				}
				// バグなどによる予期せぬエラー。
				catch (Exception ex)
				{
					command = outErr?.Invoke("予期せぬエラー:" + ex.Message);
				}

				// 実行
				yield return command;
			}
			ProgramCounter++;
		}
	}

	#region 組み込みコマンド用ヘルパーメソッド

	/// <summary>
	/// ラベル文字列を整形します．
	/// </summary>
	/// <returns>The label string.</returns>
	/// <param name="label">Label.</param>
	static string GetLabelString(string label)
	{
		// null or empty チェック
		if (string.IsNullOrEmpty(label))
			throw new ArgumentNullException(nameof(label));

		// 仕様上，#をつけてもつけなくてもよい
		if (label[0] == '#')
			label = label.Remove(0, 1);
		return label;
	}

	int GetLine(string label)
	{
		// Nullチェックと整形
		label = GetLabelString(label);

		if (!code.Labels.ContainsKey(label))
			throw new NRuntimeException($"ラベル \"{label}\"が存在しません．");
		return code.Labels[label];
	}

	/// <summary>
	/// 問答無用で文字列配列をそのまま連結します．引数の区切りがいらない場合に便利です．
	/// </summary>
	/// <returns>連結された文字列．</returns>
	/// <param name="strings">連結してほしい文字列を含む配列．</param>
	public static string CombineAll(string[] strings) => string.Join("", strings);

	#endregion

	public void Goto(int ptr) => ProgramCounter = ptr - 1;

	#region 組み込みコマンド実装
	public IEnumerator Goto(string t, params string[] args)
	{
		if (args.Length == 0)
			throw new NRuntimeException("移動先のラベルが指定されていません．");
		var label = GetLine(CombineAll(args));
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
		if (!float.TryParse(CombineAll(args), out i))
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