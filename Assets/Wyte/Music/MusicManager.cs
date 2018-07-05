using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Novel.Exceptions;
using WyteEngine.Event;
namespace WyteEngine.Music
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicManager : SingletonBaseBehaviour<MusicManager>
	{
		public MusicData[] Songs;

		AudioSource source;

		private string songName;

		public string SongName => songName;

		protected override void Awake()
		{
			base.Awake();
			source = GetComponent<AudioSource>();
		}

		void Start()
		{
			Debugger.DebugRendering += (d) => d.Append($"bgm:{songName ?? "none"} ");

			Novel.Runtime
				 .Register("bgmplay", Play)
				 .Register("bgmchange", Change)
				 .Register("bgmstop", Stop)
				 .Register("bgmstopasync", StopAsync);
		}

		public MusicData Get(string id)
		{
			var data = Songs.FirstOrDefault(m => m.Id == id);
			if (data.Clip == null)
			{
				Debug.LogWarning($"Music ID {id} doesn't exist.");
			}
			return data;
		}

		public void Play(string id)
		{
			var targetSong = Get(id);
			Stop();
			source.loop = targetSong.Loop;
			source.timeSamples = 0;
			source.clip = targetSong.Clip;
			source.volume = 1;
			source.Play();
			songName = id;
		}

		public void Play(int id)
		{
			if (Songs.Length <= id)
			{
				Debug.LogWarning($"Music ID {id} is not found. This command will be ignored.");
				return;
			}
			Play(Songs[id].Id);
		}

		public void Change(string id)
		{
			var targetSong = Get(id);
			var prevSample = source.timeSamples;
			Stop();
			source.clip = targetSong.Clip;
			source.Play();
			source.timeSamples = prevSample;
			source.loop = targetSong.Loop;
			source.volume = 1;
			songName = id;
		}

		public IEnumerator Play(string t, string[] a)
		{
			Play(NovelHelper.CombineAll(a));
			yield break;
		}

		public IEnumerator Change(string t, string[] a)
		{
			Change(NovelHelper.CombineAll(a));
			yield break;
		}

		public IEnumerator Stop(string t, string[] a)
		{
			if (a.Length == 0)
			{
				Stop();
			}
			else
			{
				float i = NovelHelper.TryParse(NovelHelper.CombineAll(a));
				yield return Stop(i);
			}
		}

		public IEnumerator StopAsync(string t, string[] a)
		{
			StartCoroutine(Stop(t, a));
			yield break;
		}

		public void Stop()
		{
			source.Stop();
			songName = null;
		}

		/// <summary>
		/// フェードアウトしながら音楽を停止します。
		/// </summary>
		/// <param name="time">フェードアウトを行う秒単位の時間。</param>
		public IEnumerator Stop(float time)
		{
			float t = 0;
			while (t < time)
			{
				t += Time.deltaTime;
				// (time - t) / time = 1 - (t / time)
				source.volume = 1 - (t / time);
				yield return null;
			}
			Stop();
			source.volume = 1;
		}
	}

	[System.Serializable]
	public struct MusicData
	{
		[SerializeField]
		private string id;
		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private bool loop;

		public string Id => id;
		public AudioClip Clip => clip;
		public bool Loop => loop;
	}
}