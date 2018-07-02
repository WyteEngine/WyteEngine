using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UObject = UnityEngine.Object;

namespace WyteEngine.Inputing
{
	public class Key
	{
		static readonly IEnumerable<KeyCode> Empty = Enumerable.Empty<KeyCode>();

		public static IEnumerable<KeyCode> GetAllKey()
		{
			if (!Input.anyKey) return Empty;
			return (Enum.GetValues(typeof(KeyCode)) as KeyCode[]).Where(p => Input.GetKey(p));
		}

		public static IEnumerable<KeyCode> GetAllKeyDown()
		{
			if (!Input.anyKeyDown) return Empty;
			return (Enum.GetValues(typeof(KeyCode)) as KeyCode[]).Where(p => Input.GetKeyDown(p));
		}
	}
}