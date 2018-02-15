using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct SfxData
{
	[SerializeField]
	private string id;
	[SerializeField]
	private AudioClip clip;

	public string Id => id;
	public AudioClip Clip => clip;
}

[RequireComponent(typeof(AudioSource))]
public class SfxManager : SingletonBaseBehaviour<SfxManager>
{

	public SfxData[] Effects;

	private AudioSource source;

	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();
		source = GetComponent<AudioSource>();
	}

	public void Play(string id)
	{
		var targetEffects = Effects.FirstOrDefault(e => e.Id == id);
		source.PlayOneShot(targetEffects.Clip);
	}

	public void Play(int id)
	{
		if (Effects.Length <= id)
		{
			Debug.LogWarning($"SFX ID {id} is not found. This command will be ignored.");
			return;
		}
		source.PlayOneShot(Effects[id].Clip);
	}

	public IEnumerator Play(string tag, string[] args)
	{
		Play(NovelHelper.CombineAll(args));
		yield break;
	}



}
