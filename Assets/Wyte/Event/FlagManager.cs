using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Novel.Exceptions;
using System.Linq;

public class FlagManager : SingletonBaseBehaviour<FlagManager>
{
	Dictionary<string, bool> flags;
	Dictionary<string, bool> skipFlags;


	protected override void Awake()
	{
		base.Awake();
		flags = new Dictionary<string, bool>();
		skipFlags = new Dictionary<string, bool>();
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
}
