using UnityEngine;
using UnityEditor;
using WyteEngine.Entities;

namespace WyteEditor.Entities
{
	public static class EntityCreateMenuExtension
	{
		const string Root = "GameObject/Entity/";
		const int priority = -50;

		[MenuItem(Root + "Sprite", false, priority)]
		static void Sprite() => CreateEntity<SpriteEntity>(nameof(SpriteEntity));

		[MenuItem(Root + "Livable", false, priority)]
		static void Livable() => CreateEntity<LivableEntity>(nameof(LivableEntity));

		[MenuItem(Root + "NPC", false, priority)]
		static void Npc() => CreateEntity<NpcBehaviour>(nameof(NpcBehaviour), "NPC");

		[MenuItem(Root + "Player", false, priority)]
		static void Player() => CreateEntity<PlayerController>(nameof(PlayerController), "Player");

		static void CreateEntity<T>(string name, string layer = default(string)) where T : Entity
		{
			var go = new GameObject(name, typeof(T));
			go.transform.parent = Selection.activeGameObject ? Selection.activeGameObject.transform : null;
			if (layer != default(string))
				go.layer = LayerMask.NameToLayer(layer);
			Selection.activeGameObject = go;
		}
	}
}