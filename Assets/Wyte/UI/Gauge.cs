using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace WyteEngine.UI
{

	[ExecuteInEditMode]
	public class Gauge : MonoBehaviour
	{

		[SerializeField]
		private Image gauge;

		[SerializeField]
		[Range(0, 1)]
		private float progress;

		public float Progress
		{
			get { return progress; }
			set { progress = value; }
		}

		// Update is called once per frame
		void Update()
		{
			if (gauge == null) return;
			Progress = Mathf.Clamp01(Progress);
			gauge.fillAmount = Progress;
		}
	}
}