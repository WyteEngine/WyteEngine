using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Novel.Exceptions;
using static NovelHelper;
using System.Linq;
using System;
using UnityEngine.Experimental.UIElements;

public class NpcManager : SingletonBaseBehaviour<NpcManager>
{
	/// <summary>
	/// マップからのエンティティ．
	/// </summary>
	List<NpcBehaviour> mappedEntity;

	/// <summary>
	/// マネージャーが管理しているエンティティ．
	/// </summary>
	List<NpcBehaviour> managedEntity;

	protected override void Awake()
	{
		base.Awake();

		mappedEntity = new List<NpcBehaviour>();
		managedEntity = new List<NpcBehaviour>();

		Map.MapChanged += (map) =>
		{
			// マップエンティティのリセット
			mappedEntity.Clear();
			mappedEntity.AddRange(FindObjectsOfType<NpcBehaviour>());
			// 管理エンティティのリセット
			managedEntity.ForEach(o => Destroy(o.gameObject));
			managedEntity.Clear();
		};
	}

	void Start()
	{
		// デーモン
		StartCoroutine(CollectGarbages());
	}

	public NpcBehaviour this[string tag] => managedEntity?.FirstOrDefault(a => a.Tag == tag) ?? mappedEntity.FirstOrDefault(a => a.Tag == tag) ?? null;

	public NpcBehaviour SpSet(string tag)
	{
		var sprite = new GameObject(string.IsNullOrWhiteSpace(tag) ? "New Sprite" : tag, typeof(NpcBehaviour)).GetComponent<NpcBehaviour>();
		sprite.Tag = tag;
		sprite.gameObject.layer = LayerMask.NameToLayer("NPC");
		return sprite;
	}

	public NpcBehaviour SpSet(string tag, string animId) => SpChr(SpSet(tag), animId);
	public NpcBehaviour SpSet(string tag, string animId, Vector2 pos) => SpOfs(SpSet(tag, animId), pos);

	public NpcBehaviour SpChr(NpcBehaviour sprite, string animId)
	{
		sprite.ChangeSprite(animId);
		return sprite;
	}

	public NpcBehaviour SpChr(string tag, string animId)
	{
		if (this[tag] == null)
			throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
		return SpChr(this[tag], animId);
	}

	public NpcBehaviour SpOfs(NpcBehaviour sprite, Vector2 pos)
	{
		sprite.transform.position = pos;
		return sprite;
	}

	public NpcBehaviour SpOfs(string tag, Vector2 pos)
	{
		if (this[tag] == null)
			throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
		return SpOfs(this[tag], pos);
	}

	public void SpClr(NpcBehaviour npc)
	{
		Destroy(npc);
	}

	public void SpClr(string tag)
	{
		if (this[tag] == null)
			throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
		SpClr(this[tag]);
	}

	#region Novel API
	// +spset <tag>, <charId>[, <x>, <y>]
	public IEnumerator SpSet(string _, string[] args)
	{
		if (args.Length < 2)
			throw new NRuntimeException("引数が足りません．");
		var spTag = args[0];
		var id = args[1];
		var pos = Vector2.zero;
		if (args.Length > 2)
		{
			if (args.Length > 4)
				throw new NRuntimeException("引数が多すぎます．");

			pos.x = TryParse(args[2]);
			pos.y = TryParse(args[3]);
		}
		SpSet(spTag, id, pos);
		yield break;
	}
	// <tag>+spofs <x>, <y>
	// +spofs <tag>, <x>, <y>
	public IEnumerator SpOfs(string tag, string[] args)
	{
		string spTag;
		NArgsAssert(args.Length >= 2);
		if (!string.IsNullOrEmpty(tag))
		{
			spTag = tag;
		}
		else
		{
			spTag = args[0];
			args = args.Skip(1).ToArray();
		}
		NArgsAssert(args.Length == 2);
		var x = TryParse(args[0]);
		var y = TryParse(args[1]);
		SpOfs(spTag, new Vector2(x, y));
		yield break;
	}

	// <tag>+spchr <animId>
	// +spchr <tag>, <animId>
	public IEnumerator SpChr(string tag, string[] args)
	{
		string spTag, animId;
		if (!string.IsNullOrEmpty(tag))
		{
			spTag = tag;
			NArgsAssert(args.Length == 1);
			animId = args[0];
		}
		else
		{
			NArgsAssert(args.Length == 2);
			spTag = args[0];
			animId = args[1];
		}
		SpChr(spTag, animId);
		yield break;
	}

	// <tag>+spclr
	// +spclr <tag>
	public IEnumerator SpClr(string tag, string[] args)
	{
		if (args.Length == 1)
			tag = args[0];
		else if (string.IsNullOrEmpty(tag))
			throw new NRuntimeException("引数が一致しません．");
		SpClr(tag);
		yield break;
	}
	#endregion

	/// <summary>
	/// バックグラウンドでEntityのガベージコレクションを行います．
	/// </summary>
	IEnumerator CollectGarbages()
	{
		while (this != null)
		{
			// new List しないと削除時に面倒な例外が出る
			foreach (var e in new List<NpcBehaviour>(mappedEntity))
			{
				if (e == null && mappedEntity.Contains(e))
					mappedEntity.Remove(e);
			}

			// new List ｓ(ry
			foreach (var e in new List<NpcBehaviour>(managedEntity))
			{
				if (e == null && managedEntity.Contains(e))
					managedEntity.Remove(e);
			}
			yield return null;
		}
	}

}
