using UnityEngine;
using System;
using System.Linq;

public class AnimationManager : SingletonBaseBehaviour<AnimationManager>
{
	[Serializable]
	public class AnimationKeyValuePair
	{
		public string Key;
		public WyteAnimation Value;
	}

	[SerializeField]
	private AnimationKeyValuePair[] animations;

	public AnimationKeyValuePair[] Animation => animations;

	public WyteAnimation this[string key]
	{
		get
		{
			var an = animations?.FirstOrDefault(a => a.Key == key)?.Value;
			if (an == null)
				Debug.LogWarning($"Animation ID {key} doesn't exist.");
			return an;
		}
	}

}