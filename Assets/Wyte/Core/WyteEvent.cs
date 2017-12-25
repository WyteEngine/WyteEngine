using UnityEngine;
using System.Collections;

/// <summary>
/// Wyte Engine Handler API．
/// </summary>
public class WyteEvent : SingletonBaseBehaviour<WyteEvent>
{
	public delegate void PlayerDeathEventHandler(Object player, Object enemy);
	public event PlayerDeathEventHandler PlayerDead;
	public event PlayerDeathEventHandler PlayerDying;

	public delegate void SaveEventHandler(GameMaster wyte);
	public event SaveEventHandler Save;
	public event SaveEventHandler GameReset;
}