using System.Collections;
using UnityEngine;
using Novel.Exceptions;
using System;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;
using UnityEngine.Serialization;

public class GameMaster : SingletonBaseBehaviour<GameMaster>
{

	/// <summary>
	/// ゲーム起動時に最初に指定するイベントのラベル名．
	/// </summary>
	[Tooltip("ゲーム内ではじめに実行するイベントのラベル名．")]
	[SerializeField]
	[FormerlySerializedAs("BootstrapLabel")]
	string bootstrapLabel;

	public string BootstrapLabel => bootstrapLabel;

	/// <summary>
	/// Gets or sets the name of the player.
	/// </summary>
	/// <value>The name of the player.</value>
	[Obsolete("Use Player.Name Instead.")]
	public string PlayerName => Player?.Name ?? "Null";

	/// <summary>
	/// プレイヤーの情報．
	/// </summary>
	/// <value>The player.</value>
	public PlayerData Player { get; private set; }

	[SerializeField]
	GameObject playerPrefab;

	[SerializeField]
	GameObject itemBar;

	[SerializeField]
	GameObject gamePad;

	public PlayerController CurrentPlayer => playerTemp == null ? null : playerTemp.GetComponent<PlayerController>();

	GameObject playerTemp;

	/// <summary>
	/// プレイヤー、NPC、すべてが動けない状態かどうか。
	/// </summary>
	/// <value><c>フリーズ状態であればtrue</c> そうでなければ<c>false</c>.
	/// </value>
	public bool IsNotFreezed { get; set; }

	private bool canMove;

	public string DebugModeHelp => "F1高速字送り{0} F2スクリプトリロード F3情報 F4フラグリセット F5 FTSデバッグ";

	[SerializeField]
	[Tooltip("Unity EditorまたはDevelopment Build時にデバッグプレイをおこなうかどうか．")]
	private bool requestDebugMode;

	public bool IsDebugMode { get; private set; }

	[SerializeField]
	private bool guiEnabled = true;

	public bool GuiEnabled
	{
		get { return guiEnabled; }
		set { guiEnabled = value; }
	}

	/// <summary>
	/// プレイヤーが移動可能かどうか。
	/// </summary>
	/// <value><c>true</c> if can move; otherwise, <c>false</c>.</value>
	public bool CanMove
	{
		get
		{
			// フリーズ状態にも依存
			return canMove && IsNotFreezed;
		}
		set
		{
			canMove = value;
			if (!value && CurrentPlayer != null)
			{
				// プレイヤーを止める
				CurrentPlayer.Velocity = new Vector2(0, CurrentPlayer.Velocity.y);
			}
		}
	}
	#region Novel API
	public IEnumerator PlayerShow(string t, string[] a)
	{
		if (playerTemp != null)
			yield break;
		float x = 0, y = 0;
		if (a.Length >= 2)
		{
			if (!float.TryParse(a[0], out x))
				throw new NRuntimeException("座標が不正です。");
			if (!float.TryParse(a[1], out y))
				throw new NRuntimeException("座標が不正です。");
		}
		playerTemp = Instantiate(playerPrefab, new Vector3(x, y), Quaternion.Euler(0, 0, 0)) as GameObject;
		PlayerShown?.Invoke(playerTemp.GetComponent<PlayerController>(), playerTemp.transform.position);
	}

	public IEnumerator PlayerHide(string t, string[] a)
	{
		if (playerTemp == null)
			yield break;
		
		var vec = playerTemp.transform.position;
		Destroy(playerTemp);
		PlayerHidden?.Invoke(playerTemp.transform.position);
		playerTemp = null;
	}

	public IEnumerator Freeze(string t, params string[] a)
	{
		if (a.Length < 1)
			throw new NRuntimeException("引数が足りません．");
		// フリーズ時に false になることに注意．
		IsNotFreezed = a[0] != "on";
		yield break;
	}

	public IEnumerator PlayerFreeze(string t, params string[] a)
	{
		if (a.Length < 1)
			throw new NRuntimeException("引数が足りません．");
		// フリーズ時に false になることに注意．
		CanMove = a[0] != "on";
		yield break;
	}

	public IEnumerator Save(string t, string[] a)
	{
		try
		{
			GameSave?.Invoke(this);
		}
		catch (Exception ex)
		{
			Debug.LogError($"セーブエラー: {ex.Message}\n{ex.StackTrace}");
			return MessageContoller.Instance.Say("", "セーブに失敗しました。");
		}
		return MessageContoller.Instance.Say("", "セーブしました。");

	}

	public IEnumerator Gui(string t, params string[] a)
	{
		NArgsAssert(a.Length == 1);
		switch (a[0].ToLower())
		{
			case "on":
				gamePad.SetActive(true);
				itemBar.SetActive(true);
				break;
			case "off":
				gamePad.SetActive(false);
				itemBar.SetActive(false);
				break;
			default:
				throw new NRuntimeException("引数にはonまたはoffを指定してください．");
		}
		yield break;
	}

	#endregion

	protected override void Awake()
	{
		base.Awake();
#if UNITY_EDITOR
		QualitySettings.vSyncCount = 1;
#else
		// Fix to 60fps
		Application.targetFrameRate = 60;
#endif
	}

	void Start()
	{
		IsNotFreezed = true;
		CanMove = true;
		// hack 後々ちゃんと書き直す
		Player = new PlayerData("ホワイト", 4);
	}

	bool booted;

	int ftsDebugCount;

	void Update()
	{
		// Novel Bootstrap
		if (!booted)
		{
			StartCoroutine(Boot());
		}

		if (IsDebugMode && Input.GetKeyDown(KeyCode.F5))
		{
			switch (ftsDebugCount)
			{
				case 0:
					Debug.Log(TextComponent.Parse(@"ようこそ $c=red;ホワイトスペース$r;へ！"));
					ftsDebugCount++;
					break;
				case 1:
					Debug.Log(TextComponent.Parse(@"君の名前は $b;$var=pname; $r;じゃな？"));
					ftsDebugCount++;
					break;
				case 2:
					Debug.Log(TextComponent.Parse(@"このメッセージは $c=#007fff;$i;TextComponent$r;のデバッグ用じゃ．"));
					ftsDebugCount++;
					break;
				case 3:
					Debug.Log(new TextComponent(@"先程は$c=blue;static$r;メソッド，今このテキストは$c=red;インスタンス$r;で生成しておるぞ．"));
					ftsDebugCount++;
					break;
				case 4:
					Debug.Log(TextComponent.Parse(@"いま諸君は， $b;$var=map_name;$r;にいるはずじゃ． $c=green;$var=bgm_name;$r;が流れていれば間違いないぞ．"));
					ftsDebugCount++;
					break;
				case 5:
					Debug.Log(new TextComponent(@"$sz=5;おーい，きこえとるか？ $r;...$sz=20;きこえとったら返事せんかー！！！$r;"));
					ftsDebugCount++;
					break;
				case 6:
					Debug.Log(new TextComponent(@"ゴホン．これでデバッグは終わりにするぞ．それでは，冒険を楽しみたまえ．"));
					ftsDebugCount = 0;
					break;
			}
		}

		// Clossplatform UI Visible
		if (GuiEnabled)
		{
			if (CanMove)
				MessageContoller.Instance.ShowBox();
			else
				MessageContoller.Instance.HideBox();
		}

		// Initialize
		if (Escape && !escaping)
		{
			StartCoroutine(Init());
		}
	}
	bool escaping;

	IEnumerator Init()
	{
		if (GuiEnabled)
		{
			escaping = true;
			yield return Freeze(null, "on");
			yield return MessageContoller.Instance.Say(null, "初期化します。");
		}

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	IEnumerator Boot()
	{
		booted = true;

		if (!IsDebugMode && requestDebugMode && ( Application.isEditor || Debug.isDebugBuild ))
		{
			IsDebugMode = true;
			yield return Freeze(null, "on");
			yield return MessageContoller.Instance?.Say(null, "デバッグモードを　起動します。\n注意! これは　開発者向けの　機能です。");
			yield return Freeze(null, "off");
		}
		
		StartCoroutine(Gui(null, GuiEnabled ? "on" : "off"));

		Novel.Run(BootstrapLabel);
	}



	//todo 必要ならescapeもキーバインドつける
	bool Escape => IsSmartDevice ? GamePadBehaviour.Instance.Get(GamePadButtons.Escape, true) : Input.GetKeyDown(KeyCode.Escape);

	public void Initalize()
	{
		if (playerTemp != null)
			Destroy(playerTemp);
		IsNotFreezed = CanMove = true;
		GameReset?.Invoke(this);
		booted = false;
	}

	public delegate void PlayerDeathEventHandler(UObject player, UObject enemy, WyteEventArgs e);
	public event PlayerDeathEventHandler PlayerDead;
	public event PlayerDeathEventHandler PlayerDying;

	public delegate void PlayerShownEventHandler(PlayerController player, Vector3 pos);
	public delegate void PlayerHiddenEventHandler(Vector3 pos);
	public event PlayerShownEventHandler PlayerShown;
	public event PlayerHiddenEventHandler PlayerHidden;

	public delegate void SaveEventHandler(GameMaster wyte);
	public event SaveEventHandler GameSave;
	public event SaveEventHandler GameReset;
}
