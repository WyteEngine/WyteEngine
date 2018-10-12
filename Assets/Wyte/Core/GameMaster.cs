using System.Collections;
using UnityEngine;
using Novel.Exceptions;
using System;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;
using UnityEngine.Serialization;
using WyteEngine.Entities;
using WyteEngine.UI.TextFormatting;
using WyteEngine.UI;
using WyteEngine.Event;
using WyteEngine.I18n;

namespace WyteEngine
{
	public class GameMaster : SingletonBaseBehaviour<GameMaster>
	{
		#region ゲームデータ
		public readonly string LongVersion = "1.0.0";
		public readonly string ShortVersion = "100";

		[SerializeField] string gameVersion;
		public string GameVersion => gameVersion;

		#endregion

		#region インスペクター変数
		[Tooltip("ゲーム内ではじめに実行するイベントのラベル名．")]
		[FormerlySerializedAs("BootstrapLabel")]
		[SerializeField]　string bootstrapLabel;

		[SerializeField] GameObject playerPrefab;

		[SerializeField] GameObject itemBar;

		[SerializeField] GameObject gamePad;
		#endregion

		GameObject playerTemp;

		private bool canMove;
		
		bool escaping;

		private IEnumerator FtsDebug { get; set; }

		public PlayerController CurrentPlayer => playerTemp == null ? null : playerTemp.GetComponent<PlayerController>();

		public string BootstrapLabel => bootstrapLabel;

		public PlayerData Player { get; private set; }

		/// <summary>
		/// プレイヤー、NPC、すべてが動けない状態かどうか。
		/// </summary>
		/// <value><c>フリーズ状態であればtrue</c> そうでなければ<c>false</c>.</value>
		public bool IsNotFreezed { get; set; }


		public string DebugModeHelp => I18n["system.debug.help"];

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

			Novel.Runtime
				 .Register("pshow", PlayerShow)
				 .Register("phide", PlayerHide)
				 .Register("gui", Gui)
				 .Register("freeze", Freeze)
				 .Register("pfreeze", PlayerFreeze);
		}

		protected override void PostStart()
		{
			StartCoroutine(Boot());
		}

		protected override void Update()
		{
			base.Update();
			// FTS デバッグ
			if (IsDebugMode && Input.GetKeyDown(KeyCode.F5))
			{
				if (FtsDebug == null)
					FtsDebug = DoFtsDebug();
				// コルーチンを逐次呼ぶ
				FtsDebug.MoveNext();
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
			if (Escape)
			{
				if (!ConfigController.instance.IsVisible)
					ConfigController.Instance.Show();
				else
					ConfigController.Instance.Back();
			}
		}

		IEnumerator DoFtsDebug()
		{
			var count = 0;
			while (true)
			{
				switch (count)
				{
					case 0:
						Debug.Log(TextComponent.Parse(@"ようこそ $c=red;ホワイトスペース$r;へ！"));
						break;
					case 1:
						Debug.Log(TextComponent.Parse(@"君の名前は $b;$var=pname; $r;じゃな？"));
						break;
					case 2:
						Debug.Log(TextComponent.Parse(@"このメッセージは $c=#007fff;$i;TextComponent$r;のデバッグ用じゃ．"));
						break;
					case 3:
						Debug.Log(new TextComponent(@"先程は$c=blue;static$r;メソッド，今このテキストは$c=red;インスタンス$r;で生成しておるぞ．"));
						break;
					case 4:
						var text = TextComponentBuilder.Create()
										.Text("これは")
										.Color(Color.blue)
										.Bold()
										.Italic()
										.Text(nameof(TextComponentBuilder))
										.Reset()
										.Text("を使って")
										.Size(15)
										.Text("生成しているぞ！")
										.CurrentText;
						Debug.Log(text);
						break;
					case 5:
						Debug.Log(TextComponent.Parse(@"いま諸君は， $b;$var=map_name;$r;にいるはずじゃ． $c=green;$var=bgm_name;$r;が流れていれば間違いないぞ．"));
						break;
					case 6:
						Debug.Log(new TextComponent(@"$sz=5;おーい，きこえとるか？ $r;...$sz=20;きこえとったら返事せんかー！！！$r;"));
						break;
					case 7:
						Debug.Log(new TextComponent(@"ゴホン．これでデバッグは終わりにするぞ．それでは，冒険を楽しみたまえ．"));
						break;
				}
				count++;
				if (count > 7)
					count = 0;
				yield return null;
			}
		}


		[RuntimeInitializeOnLoadMethod]
		static void OnLoad()
		{
#if UNITY_STANDALONE
			Screen.SetResolution(640, 360, false, 60);
#endif
		}
		
		/// <summary>
		/// 起動処理を行います。
		/// </summary>
		IEnumerator Boot()
		{
			// デバッグモードの開始
			if (!IsDebugMode && requestDebugMode && (Application.isEditor || Debug.isDebugBuild))
			{
				IsDebugMode = true;
			}

			yield return Gui(null, GuiEnabled ? "on" : "off");

			Novel.Run(BootstrapLabel);
		}

		//todo 必要ならescapeもキーバインドつける
		public bool Escape => IsSmartDevice ? GamePadBehaviour.Instance.Get(GamePadButtons.Escape, true) : Input.GetKeyDown(KeyBind.Pause);

		/// <summary>
		/// プレイヤーが死んだときに呼ばれます。
		/// </summary>
		public void Initalize()
		{
			if (playerTemp != null)
				Destroy(playerTemp);
			Player.Life = Player.MaxLife;
			Player.WaterPoint = PlayerData.MaxWaterPoint;
			Player.FoodPoint = PlayerData.MaxFoodPoint;
			IsNotFreezed = CanMove = true;
			GameReset?.Invoke(this);
			IsPostInitialized = false;
		}

		public void Reset()
		{
			SceneManager.LoadScene("Main");
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

}