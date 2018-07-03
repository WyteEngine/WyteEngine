using UnityEngine;
using System.Collections;
using WyteEngine.Entities;

namespace WyteEngine.UI
{
	public class BossGaugeBinder : SingletonBaseBehaviour<BossGaugeBinder>
	{
		[SerializeField]
		private Gauge gauge;

		public Gauge Gauge
		{
			get { return gauge; }
			set { gauge = value; }
		}

		private Entity bindedEntity;
		public int Value { get; set; }
		public int MaxValue { get; set; }
		public float ActualValue => MaxValue != 0 ? (float)Value / MaxValue : 0;

		public void Show()
		{
			Gauge.gameObject.SetActive(true);
		}

		public void Hide()
		{
			Gauge.gameObject.SetActive(false);
		}

		public void Bind(Entity entity)
		{
			bindedEntity = entity;
		}

		public void Bind(string tag)
		{
			bindedEntity = Npc[tag];
		}

		public void UnBind()
		{
			bindedEntity = null;
		}

		#region API
		public IEnumerator Show(string _, string[] __)
		{
			Show();
			yield break;
		}

		public IEnumerator Hide(string _, string[] __)
		{
			Hide();
			yield break;
		}

		public IEnumerator Bind(string _, string[] args)
		{
			NArgsAssert(args.Length == 1);
			Bind(args[0]);
			yield break;
		}

		public IEnumerator UnBind(string _, string[] args)
		{
			NArgsAssert(args.Length == 1);
			UnBind();
			yield break;
		}

		public IEnumerator SetMaxValue(string _, string[] args)
		{
			int mv;
			NArgsAssert(args.Length == 1);
			NArgsAssert(int.TryParse(args[0], out mv), 0);
			MaxValue = mv;
			yield break;
		}

		public IEnumerator SetValue(string _, string[] args)
		{
			int v;
			NArgsAssert(args.Length == 1);
			NArgsAssert(int.TryParse(args[0], out v), 0);
			Value = v;
			yield break;
		}
		#endregion

		#region UnityMessage
		private void Start()
		{
			Hide();
		}

		private void Update()
		{
			if (Gauge == null)
			{
				Debug.LogError("Attach a gauge prefab to a BossGaugeBinder.");
				enabled = false;
			}
			if (bindedEntity != null)
			{
				Value = bindedEntity.Health;
				MaxValue = bindedEntity.MaxHealth;
			}

			Gauge.Progress = ActualValue;
		}
		#endregion
	}
}