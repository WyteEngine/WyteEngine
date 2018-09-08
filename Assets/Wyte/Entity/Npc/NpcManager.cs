using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Novel.Exceptions;
using System.Linq;
using WyteEngine.Event;
using static WyteEngine.Entities.SpriteEntity;
using System;

namespace WyteEngine.Entities
{
	public class NpcManager : SingletonBaseBehaviour<NpcManager>
	{
		/// <summary>
		/// マップからのエンティティ．
		/// </summary>
		List<LivableEntity> mappedEntity;

		/// <summary>
		/// マネージャーが管理しているエンティティ．
		/// </summary>
		List<LivableEntity> managedEntity;

		protected override void Awake()
		{
			base.Awake();

			mappedEntity = new List<LivableEntity>();
			managedEntity = new List<LivableEntity>();

			Map.MapChanged += (map) =>
			{
				// マップエンティティのリセット
				mappedEntity.Clear();
				mappedEntity.AddRange(FindObjectsOfType<LivableEntity>());
				// 管理エンティティのリセット
				managedEntity.ForEach(o =>
					{
						if (o != null)
							Destroy(o.gameObject);
					});
				managedEntity.Clear();
			};

		}

		void Start()
		{
			// デーモン
			StartCoroutine(CollectGarbages());
			Wyte.PlayerShown += (player, pos) =>
			{
				managedEntity.Add(player);
			};

			Novel.Runtime
				 .Register("spset", SpSet)
				 .Register("spsetf", SpSetF)
				 .Register("spofs", SpOfs)
				 .Register("spchr", SpChr)
				 .Register("spclr", SpClr)
				 .Register("spwalk", SpWalk)
				 .Register("speve", SpEvent)
				 .Register("spdir", SpDir)
				 .Register("spwalkto", SpWalkTo);
		}

		public LivableEntity this[string tag]
		{
			get
			{
				var m = managedEntity?.FirstOrDefault(a => a.Tag == tag) ?? mappedEntity.FirstOrDefault(a => a.Tag == tag);
				if (m == null)
				{
					Debug.LogWarning($"NPC ID {tag} は見つかりませんでした．");
				}
				return m;
			}
		}

		public LivableEntity SpSet(string tag)
		{
			var sprite = new GameObject(string.IsNullOrWhiteSpace(tag) ? "New Sprite" : tag, typeof(NpcBehaviour)).GetComponent<NpcBehaviour>();
			sprite.GroundLayer = 1 << LayerMask.NameToLayer("Ground");
			sprite.IsManagedNpc = true;
			sprite.Tag = tag;
			sprite.gameObject.layer = LayerMask.NameToLayer("NPC");
			sprite.GetComponent<Collider2D>().isTrigger = true;
			managedEntity.Add(sprite);
			return sprite;
		}

		public LivableEntity SpSet(string tag, string animId) => SpChr(SpSet(tag), animId);
		public LivableEntity SpSet(string tag, string animId, Vector2 pos) => SpOfs(SpSet(tag, animId), pos);

		public LivableEntity SpChr(LivableEntity sprite, string animId)
		{
			sprite.ChangeSprite(animId);
			return sprite;
		}

		public LivableEntity SpChr(string tag, string animId)
		{
			if (this[tag] == null)
				throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
			return SpChr(this[tag], animId);
		}

		public LivableEntity SpOfs(LivableEntity sprite, Vector2 pos)
		{
			sprite.transform.position = pos;
			return sprite;
		}

		public LivableEntity SpOfs(string tag, Vector2 pos)
		{
			if (this[tag] == null)
				throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
			return SpOfs(this[tag], pos);
		}

		public LivableEntity SpDir(string tag, SpriteDirection dir)
		{
			if (this[tag] == null)
				throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
			return SpDir(this[tag], dir);
		}

		private LivableEntity SpDir(LivableEntity sprite, SpriteDirection dir)
		{
			sprite.Direction = dir;
			return sprite;
		}

		public void SpClr(LivableEntity npc)
		{
			Destroy(npc.gameObject);
		}

		public void SpClr(string tag)
		{
			if (this[tag] == null)
				throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
			SpClr(this[tag]);
		}

		public NpcBehaviour SpEvent(NpcBehaviour sprite, string label)
		{
			sprite.Label = label;
			return sprite;
		}

		public NpcBehaviour SpEvent(string tag, string eventId)
		{
			if (this[tag] == null)
				throw new NRuntimeException($"NPC tag:{tag} は存在しません．");
			if (!(this[tag] is NpcBehaviour))
				throw new NRuntimeException($"tag:{tag} はNPC Entityではありません．");
			return SpEvent(this[tag] as NpcBehaviour, eventId);
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

				pos.x = NovelHelper.TryParse(args[2]);
				pos.y = NovelHelper.TryParse(args[3]);
			}
			SpSet(spTag, id, pos);
			yield break;
		}

		// +spsetf <tag>, <charId>[, <x>, <y>]
		public IEnumerator SpSetF(string _, string[] args)
		{
			yield return SpSet(_, args);
			(this[args[0]] as NpcBehaviour).GravityScaleMultiplier = 0;
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
			var x = NovelHelper.TryParse(args[0]);
			var y = NovelHelper.TryParse(args[1]);
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

		// <tag>+spchr <animId>
		// +spchr <tag>, <animId>
		public IEnumerator SpEvent(string tag, string[] args)
		{
			string spTag, eventId;
			if (!string.IsNullOrEmpty(tag))
			{
				spTag = tag;
				NArgsAssert(args.Length == 1);
				eventId = args[0];
			}
			else
			{
				NArgsAssert(args.Length == 2);
				spTag = args[0];
				eventId = args[1];
			}
			SpEvent(spTag, eventId);
			yield break;
		}

		// <tag>+spchr <animId>
		// +spchr <tag>, <animId>
		public IEnumerator SpDir(string tag, string[] args)
		{
			string spTag, dir;
			if (!string.IsNullOrEmpty(tag))
			{
				spTag = tag;
				NArgsAssert(args.Length == 1);
				dir = args[0];
			}
			else
			{
				NArgsAssert(args.Length == 2);
				spTag = args[0];
				dir = args[1];
			}
			dir = dir.ToLower();
			if (dir != "left" && dir != "right")
			{
				throw new NRuntimeException("Direction がおかしいです。");
			}
			SpDir(spTag, dir == "left" ? SpriteDirection.Left : SpriteDirection.Right);
			yield break;
		}

		// <tag>+spwalk <distance>, <time>
		// +spwalk <tag>, <distance>, <time>
		public IEnumerator SpWalk(string tag, string[] args)
		{
			string spTag;
			float distance, time;
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
			var npc = this[spTag];
			if (npc == null)
				throw new NRuntimeException($"NPC tag:{tag} は存在しません．");

			NArgsAssert(args.Length == 2);

			distance = NovelHelper.TryParse(args[0]);
			time = NovelHelper.TryParse(args[1]);
			yield return Walk(npc, distance, time);
		}
		// <tag>+spwalk <x>, <time>
		// +spwalk <tag>, <x>, <time>
		public IEnumerator SpWalkTo(string tag, string[] args)
		{
			string spTag;
			float distance, time, x;
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
			var npc = this[spTag];
			if (npc == null)
				throw new NRuntimeException($"NPC tag:{tag} は存在しません．");

			NArgsAssert(args.Length == 2);

			x = NovelHelper.TryParse(args[0]);
			time = NovelHelper.TryParse(args[1]);

			yield return Walk(npc, x - npc.transform.position.x, time);
		}

		private IEnumerator Walk(LivableEntity npc, float distance, float time)
		{
			// 速さ = 距離 / 時間
			var speed = distance / time;
			if (float.IsNaN(speed) || float.IsInfinity(speed))
				yield break;
			npc.Move(speed);
			yield return new WaitForSeconds(time);
			npc.Move(0);
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
				foreach (var e in new List<LivableEntity>(mappedEntity))
				{
					if (e == null && mappedEntity.Contains(e))
						mappedEntity.Remove(e);
				}

				// new List ｓ(ry
				foreach (var e in new List<LivableEntity>(managedEntity))
				{
					if (e == null && managedEntity.Contains(e))
						managedEntity.Remove(e);
				}
				yield return null;
			}
		}

	}
}