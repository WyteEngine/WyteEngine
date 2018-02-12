using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Novel.Exceptions;
using System.Linq;
using FlagMap = System.Collections.Generic.Dictionary<string, bool>;

public class FlagManager : SingletonBaseBehaviour<FlagManager>
{
	/// <summary>
	/// セーブされるフラグ．
	/// </summary>
	FlagMap flags;
	/// <summary>
	/// セーブされず，ゲームを終了すると消去されるフラグ．
	/// </summary>
	FlagMap skipFlags;
	/// <summary>
	/// マップ切り替え時に消去されるスキップフラグ．
	/// </summary>
	FlagMap areaFlags;


	protected override void Awake()
	{
		base.Awake();
		flags = SaveDataHelper.Load<FlagMap>("flag.json") ?? new FlagMap();
		skipFlags = new FlagMap();
		areaFlags = new FlagMap();

		WyteEvent.Instance.Save += (wyte) =>
		{
			SaveDataHelper.Save("flag.json", flags);
			// スキップフラグはセーブされない．
		};

		WyteEvent.Instance.MapChanged += (wyte) =>
		{
			// さようなら．
			areaFlags.Clear();
		};

	}

	private void FlagImpl(Dictionary<string, bool> flagDic, string[] args)
	{
		if (args.Length < 2)
			throw new NRuntimeException("引数が足りません。");
		flagDic[args[1]] = args[0] == "on";
	}

	private IEnumerator OnFlagImpl(Dictionary<string, bool> flagDic, string key, string[] labels)
	{
		if (labels.Length < 1)
			return null;
		if (flagDic.ContainsKey(key) && flagDic[key])
			return Novel.Runtime.Goto("", labels[0]);
		if (labels.Length >= 2)
			return Novel.Runtime.Goto("", labels[1]);
		return null;
	
	}

	public IEnumerator Flag(string _, string[] args)
	{
		FlagImpl(flags, args);
		yield break;
	}

	public IEnumerator OnFlag(string name, string[] labels)
	{
		yield return OnFlagImpl(flags, name, labels);
	}


	public IEnumerator SkipFlag(string _, string[] args)
	{
		FlagImpl(skipFlags, args);
		yield break;
	}

	public IEnumerator OnSkipFlag(string name, string[] labels)
	{
		yield return OnFlagImpl(skipFlags, name, labels);
	}

	public IEnumerator AreaFlag(string _, string[] args)
	{
		FlagImpl(areaFlags, args);
		yield break;
	}

	public IEnumerator OnAreaFlag(string name, string[] labels)
	{
		yield return OnFlagImpl(areaFlags, name, labels);
	}
}
