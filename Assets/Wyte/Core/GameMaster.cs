using System.Collections;
using UnityEngine;
using Novel.Exceptions;

public class GameMaster : SingletonBaseBehaviour<GameMaster>
{

	/// <summary>
	/// ゲーム起動時に最初に指定するイベントのラベル名．
	/// </summary>
	[Tooltip("ゲーム内ではじめに実行するイベントのラベル名．")]
	[SerializeField]
	public string BootstrapLabel;

	/// <summary>
	/// Gets or sets the name of the player.
	/// </summary>
	/// <value>The name of the player.</value>
	[System.Obsolete("Use Player.Name Instead.")]
	public string PlayerName => Player?.Name ?? "Null";

	/// <summary>
	/// プレイヤーの情報．
	/// </summary>
	/// <value>The player.</value>
	public PlayerData Player { get; private set; }

	[SerializeField]
	GameObject playerPrefab;

	GameObject playerTemp;

	/// <summary>
	/// プレイヤー、NPC、すべてが動けない状態かどうか。
	/// </summary>
	/// <value><c>フリーズ状態であればtrue</c> そうでなければ<c>false</c>.
	/// </value>
	public bool IsNotFreezed { get; set; }

	private bool canMove;

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
		}
	}

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
	}

	public IEnumerator PlayerHide(string t, string[] a)
	{
		if (playerTemp != null)
			Destroy(playerTemp);
		yield break;
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
		Debug.Log(canMove);
		yield break;
	}

	void Start()
	{
		IsNotFreezed = true;
		CanMove = true;
		// hack 後々ちゃんと書き直す
		Player = new PlayerData("ホワイト", 20);
	}

	private bool booted;

	void Update()
	{
		// Novel Bootstrap
		if (!booted)
		{
			Novel.Run(BootstrapLabel);
			booted = true;
		}


	}
}

public class PlayerData
{
	public string Name { get; set; }
	public int Life { get; set; }
	public int MaxLife { get; set; }

	public PlayerData(string name, int life, int mlife)
	{
		Name = name;
		Life = life;
		MaxLife = mlife;
	}

	public PlayerData(string name, int maxLife) : this(name, maxLife, maxLife) { }
}