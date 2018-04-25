using UnityEngine;
using System.Collections;
using System;

public class DemoInitializer : SingletonBaseBehaviour<DemoInitializer>
{
	protected override void Awake()
	{
		base.Awake();

		ItemMan.InitializeItem += InitalizeItem;
	}

	private void InitalizeItem(ItemManager manager)
	{
		manager
			.Add("clock", new ItemUsableKey("clock", "時計", "house_clock", "現在時刻がわかる　文明の利器。"))
			.Add("key", new ItemKey("key", "家のカギ", "家のドアを　開けられるカギ。"));
	}
}
