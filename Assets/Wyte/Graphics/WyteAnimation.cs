using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[Serializable]
[CreateAssetMenu]
public class WyteAnimation : ScriptableObject
{
	[SerializeField]
	List<AnimData> animationList;
	public List<AnimData> AnimationList
	{
		get { return animationList; }
		set { animationList = value; }
	}

	[SerializeField]
	bool useLoop;
	public bool UseLoop
	{
		get { return useLoop; }
		set { useLoop = value; }
	}

	[SerializeField]
	int loopTimes;
	public int LoopTimes
	{
		get { return loopTimes; }
		set { loopTimes = value; }
	}

	public AnimData this[int index] => AnimationList != null && index < AnimationList.Count ? AnimationList[index] : null;

	public int Count => AnimationList.Count;

	[Serializable]
	public class AnimData
	{
		[SerializeField]
		Sprite sprite;
		public Sprite Sprite
		{
			get { return sprite; }
			set { sprite = value; }
		}

		[SerializeField]
		float time;
		public float Time
		{
			get { return time; }
			set { time = value; }
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(WyteAnimation))]
public class WyteAnimationEditor : Editor
{
	WyteAnimation Anim => target as WyteAnimation;

	ReorderableList list;

	const float elementHeight = 64;

	float fillTime;

	public void OnEnable()
	{
		if (Anim.AnimationList == null)
			Anim.AnimationList = new List<WyteAnimation.AnimData>();

		list = new ReorderableList(Anim.AnimationList, typeof(WyteAnimation.AnimData), true, true, true, true);
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

			EditorGUI.LabelField(labelRect, "Time");
			Anim.AnimationList[index].Time = EditorGUI.FloatField(fieldRect, Anim.AnimationList[index].Time);
			Anim.AnimationList[index].Sprite = EditorGUI.ObjectField(spriteRect, Anim.AnimationList[index].Sprite, typeof(Sprite), false) as Sprite;


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
		Anim.UseLoop = EditorGUILayout.Toggle("Loop", Anim.UseLoop);
		if (Anim.UseLoop)
		{
			Anim.LoopTimes = EditorGUILayout.IntField("Loop Times (0 if ∞)", Anim.LoopTimes);
		}
		EditorGUILayout.Space();
		if (list != null && Anim.AnimationList != null)
		{
			list.DoLayoutList();
		}
		fillTime = EditorGUILayout.FloatField("Time (Batch input)", fillTime);
		if (GUILayout.Button("Set Time"))
		{
			foreach (var anim in Anim.AnimationList)
				anim.Time = fillTime;
		}
		if (EditorGUI.EndChangeCheck())
		{
			SaveAsset();
		}
	}

}

#endif
