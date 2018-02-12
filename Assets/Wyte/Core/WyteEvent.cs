using UnityEngine;
using System.Collections;

/// <summary>
/// Wyte Engine Handler API．
/// </summary>
public class WyteEvent : SingletonBaseBehaviour<WyteEvent>
{
	public delegate void PlayerDeathEventHandler(Object player, Object enemy);
	public PlayerDeathEventHandler PlayerDead;
	public PlayerDeathEventHandler PlayerDying;

	public delegate void SaveEventHandler(GameMaster wyte);
	public SaveEventHandler Save;
	public SaveEventHandler GameReset;

	public delegate void MapChangedEventHandler(MapProperty map);
	public MapChangedEventHandler MapChanged;

}