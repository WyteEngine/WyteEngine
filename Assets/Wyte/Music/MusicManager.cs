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
			     .Register("bgmstopasync", StopAsync)
				 .Register("bgmwait", Wait);
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
			waitCache = 0;
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

		private double waitCache;

		private double waitQueue;

		public IEnumerator Wait(string t, string[] a)
		{
			// wait		0.5
			// process	0.2
			// wait		0.3
			// process	0.7
			// wait		-0.2	skip
			// process	
			float i;
			if (!float.TryParse(NovelHelper.CombineAll(a), out i))
				throw new NRuntimeException("不正な数値です．");
			if (!source.isPlaying)
			{
				yield return Novel.Runtime.Wait(t, a);
				waitCache = 0;
			}
			else
			{
				if (waitCache != 0)
				{
					var wt = i - (source.time - waitCache) + waitQueue;
					var time = source.time;
					Debug.Log($"wt:{wt}\nwaitCache:{waitCache}\nsource.time:{source.time}\nwaitQueue:{waitQueue}");
					waitQueue = 0;
					if (wt < 0)
					{
						waitQueue = wt;
						waitCache = source.time;
					}
					else
					{
						var prevTime = source.time;
						while (source.time - prevTime < wt) yield return new WaitForEndOfFrame();
						waitCache = prevTime + wt;
					}
				}
				else
				{
					Debug.Log("waitCache == 0");
					var prevTime = source.time;
					while (source.time - prevTime < i) yield return new WaitForEndOfFrame();
					waitCache = prevTime + i;
				}
			}
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
				yield return new WaitForEndOfFrame();
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