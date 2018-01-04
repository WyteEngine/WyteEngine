using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC なオブジェクトにアタッチします。
/// </summary>
public class NpcBehaviour : BaseBehaviour {

	[SerializeField]
	[Tooltip("アクション時に実行するイベント ラベル。")]
	private string label;

	public string Label => label;

	[SerializeField]
	[Tooltip("イベントの発火条件。")]
	private EventCondition eventWhen;

	public EventCondition EventWhen => eventWhen;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

/// <summary>
/// Novel イベントの発生条件。
/// </summary>
public enum EventCondition
{
	/// <summary>
	/// 話しかけた時。
	/// </summary>
	Talked,
	/// <summary>
	/// イベントを実行しません。
	/// </summary>
	None,
	/// <summary>
	/// プレイヤーが触れた時。
	/// </summary>
	Touched,
	/// <summary>
	/// 攻撃された時。
	/// </summary>
	Punched
}