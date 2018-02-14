using UnityEngine;
using UnityEditor;
using System.Collections;

public class EntityCreateMenuExtension
{
	const string Root = "GameObject/Entity/";
	const int priority = -100;

	[MenuItem(Root + "Sprite", false, priority)]
	static void Sprite() => CreateEntity<SpriteEntity>(nameof(SpriteEntity));

	[MenuItem(Root + "Livable", false, priority)]
	static void Livable() => CreateEntity<LivableEntity>(nameof(LivableEntity));

	[MenuItem(Root + "NPC", false, priority)]
	static void Npc() => CreateEntity<NpcBehaviour>(nameof(NpcBehaviour));

	[MenuItem(Root + "Player", false, priority)]
	static void Player() => CreateEntity<PlayerController>(nameof(PlayerController));

	static void CreateEntity<T>(string name) where T : Entity
	{
		var go = new GameObject(name, typeof(T));
		go.transform.parent = Selection.activeGameObject ? Selection.activeGameObject.transform : null;
		Selection.activeGameObject = go;
	}
}
