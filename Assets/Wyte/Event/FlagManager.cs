using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Novel.Exceptions;

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

	private void OnFlagImpl(Dictionary<string, bool> flagDic, string key, string[] labels)
	{
		if (labels.Length < 1)
			return;
		if (flagDic.ContainsKey(key) && flagDic[key]) 
			EventController.Instance.Runtime.Goto("", labels[0]);
		else if (labels.Length >= 2)
			EventController.Instance.Runtime.Goto("", labels[1]);
	
	}

	public IEnumerator Flag(string _, string[] args)
	{
		FlagImpl(flags, args);
		yield break;
	}

	public IEnumerator OnFlag(string name, string[] labels)
	{
		OnFlagImpl(flags, name, labels);
		yield break;
	}


	public IEnumerator SkipFlag(string _, string[] args)
	{
		FlagImpl(skipFlags, args);
		yield break;
	}

	public IEnumerator OnSkipFlag(string name, string[] labels)
	{
		OnFlagImpl(skipFlags, name, labels);
		yield break;
	}
}
