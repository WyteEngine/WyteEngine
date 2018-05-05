using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class ItemManager : SingletonBaseBehaviour<ItemManager>
{
	[SerializeField]
	List<KeyValuePair<string, Sprite>> textures;

	public List<KeyValuePair<string, Sprite>> Textures
	{
		get { return textures; }
		set { textures = value; }
	}

	public Dictionary<string, IItem> Items { get; private set; }

	// Use this for initialization
	void Start()
	{
		Items = new Dictionary<string, IItem>();

		OnInitializeItem();
	}

	void OnInitializeItem()
	{
		InitializeItem?.Invoke(this);
	}

	public ItemManager Add(string id, IItem theItem)
	{
		if (theItem == null)
			throw new ArgumentNullException(nameof(theItem));

		if (Items.ContainsKey(id))
			throw new ArgumentException($"重複するID {id} です．");

		Items[id] = theItem;

		return this;
	}

	public IItem this[string key] => Items.ContainsKey(key) ? Items[key] : default(IItem);

	public event InitializeItemEventHandler InitializeItem;

	public delegate void InitializeItemEventHandler(ItemManager manager);
}



#if UNITY_EDITOR
[CustomEditor(typeof(ItemManager))]
public class ItemManagerEditor : Editor
{
	ItemManager Manager => target as ItemManager;

	ReorderableList list;

	const float elementHeight = 64;

	float fillTime;

	public void OnEnable()
	{
		if (Manager.Textures == null)
			Manager.Textures = new List<KeyValuePair<string, Sprite>>();

		list = new ReorderableList(Manager.Textures, typeof(Sprite), true, true, true, true);
		list.drawHeaderCallback = (rect) =>
		{
			GUI.Label(rect, "Animation Sets");
		};

		list.drawElementCallback = (rect, index, isActive, isFocused) =>
		{
			EditorGUI.BeginChangeCheck();

			var labelRect = new Rect(rect.x + 8, rect.y + 8, 32, 16);
			var fieldRect = new Rect(rect.x + 48, rect.y + 8, 30, 16);
			var spriteRect = new Rect(rect.xMax - 56, rect.y + 8, 48, 48);

			EditorGUI.LabelField(labelRect, "ID");
			Manager.Textures[index] = new KeyValuePair<string, Sprite>
			(
				EditorGUI.TextField(fieldRect, Manager.Textures[index].Key),
				EditorGUI.ObjectField(spriteRect, Manager.Textures[index].Value, typeof(Sprite), false) as Sprite
			);

			if (EditorGUI.EndChangeCheck())
			{
				SaveAsset();
			}
		};

		list.elementHeight = elementHeight;
		list.onReorderCallback = (list) => SaveAsset();
	}

	void SaveAsset()
	{
		EditorUtility.SetDirty(target);
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

		if (list != null && Manager.Textures != null)
		{
			list.DoLayoutList();
		}

		if (EditorGUI.EndChangeCheck())
		{
			SaveAsset();
		}
	}

}

#endif
