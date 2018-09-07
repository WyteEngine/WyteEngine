using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Novel.Exceptions;
using System.Linq;
using FlagMap = WyteEngine.Event.FlagDictionary;
using WyteEngine.Helper;
using System;

namespace WyteEngine.Event
{
	public class FlagManager : SingletonBaseBehaviour<FlagManager>
	{
		public FlagMap Flags { get; private set; }
		public FlagMap SkipFlags { get; private set; }
		public FlagMap AreaFlags { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			Flags = SaveDataHelper.Load<FlagMap>("flag.json") ?? new FlagMap();
			SkipFlags = new FlagMap();
			AreaFlags = new FlagMap();

			Wyte.GameSave += (wyte) =>
			{
				SaveDataHelper.Save("flag.json", Flags);
			// スキップフラグはセーブされない．
			};

			Map.MapChanged += (wyte) =>
			{
				// さようなら．
				AreaFlags.Clear();
			};
		}

		private void Start()
		{
			Novel.Runtime
 				 .Register("flag", Flag)
 				 .Register("onflag", OnFlag)
				 .Register("sflag", SkipFlag)
				 .Register("onsflag", OnSkipFlag)
				 .Register("aflag", AreaFlag)
				 .Register("onaflag", OnAreaFlag)
				 .Register("clearallflag", ClearAllFlag);
		}

		protected override void Update()
		{
			base.Update();
			if (Wyte.IsDebugMode && Input.GetKeyDown(KeyCode.F4))
			{
				Flags.Clear();
				SkipFlags.Clear();
				AreaFlags.Clear();
				Debug.Log("<color=yellow>フラグを削除しました．</color>", this);
			}
		}

		void FlagImpl(FlagMap flagDic, string[] args)
		{
			if (args.Length < 2)
				throw new NRuntimeException("引数が足りません。");
			flagDic[args[1]] = args[0] == "on";
		}

		private IEnumerator OnFlagImpl(FlagMap flagDic, string key, string[] labels)
		{
			if (labels.Length < 1)
				return null;
			var useGosub = false;
			if (labels.Length >= 3 || labels[0].ToLower() == "goto" || labels[0].ToLower() == "gosub")
			{
				useGosub = labels[0].ToLower() == "gosub";
				labels = labels.Skip(1).ToArray();
			}
			if (flagDic[key])
				return useGosub ? Novel.Runtime.Gosub("", labels[0]) : Novel.Runtime.Goto("", labels[0]);
			if (labels.Length >= 2)
				return useGosub ? Novel.Runtime.Gosub("", labels[1]) : Novel.Runtime.Goto("", labels[1]);
			return null;

		}

		public new IEnumerator Flag(string _, string[] args)
		{
			FlagImpl(Flags, args);
			yield break;
		}

		public IEnumerator OnFlag(string name, string[] labels)
		{
			yield return OnFlagImpl(Flags, name, labels);
		}


		public IEnumerator SkipFlag(string _, string[] args)
		{
			FlagImpl(SkipFlags, args);
			yield break;
		}

		public IEnumerator OnSkipFlag(string name, string[] labels)
		{
			yield return OnFlagImpl(SkipFlags, name, labels);
		}

		public IEnumerator AreaFlag(string _, string[] args)
		{
			FlagImpl(AreaFlags, args);
			yield break;
		}

		public IEnumerator OnAreaFlag(string name, string[] labels)
		{
			yield return OnFlagImpl(AreaFlags, name, labels);
		}


		public IEnumerator ClearAllFlag(string name, string[] args)
		{
			Flags.Clear();
			SkipFlags.Clear();
			AreaFlags.Clear();
			yield break;
		}

	}

	public sealed class FlagDictionary : IDictionary<string, bool>
	{
		private IDictionary<string, bool> dic;

		public FlagDictionary()
		{
			dic = new Dictionary<string, bool>();
		}

		public bool this[string key]
		{
			get { return dic.ContainsKey(key) && dic[key]; }
			set { dic[key] = value; }
		}

		public ICollection<string> Keys => dic.Keys;

		public ICollection<bool> Values => dic.Values;

		public int Count => dic.Count;

		public bool IsReadOnly => false;

		public void Add(string key, bool value) => dic[key] = value;

		public void Add(KeyValuePair<string, bool> item) => dic[item.Key] = item.Value;

		public void Clear() => dic.Clear();

		public bool Contains(KeyValuePair<string, bool> item) => dic.Contains(item);

		public bool ContainsKey(string key) => dic.ContainsKey(key);

		public void CopyTo(KeyValuePair<string, bool>[] array, int arrayIndex) => dic.CopyTo(array, arrayIndex);

		public IEnumerator<KeyValuePair<string, bool>> GetEnumerator() => dic.GetEnumerator();

		public bool Remove(string key) => dic.Remove(key);

		public bool Remove(KeyValuePair<string, bool> item) => dic.Remove(item);

		public bool TryGetValue(string key, out bool value) => dic.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}