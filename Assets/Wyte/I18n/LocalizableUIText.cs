using UnityEngine;
using UnityEngine.UI;

namespace WyteEngine.I18n
{
	[RequireComponent(typeof(Text))]
	public class LocalizableUIText : BaseBehaviour
	{
		[SerializeField]
		private string localizableStringId;


		Text ui;

		public string LocalizableStringId
		{
			get { return localizableStringId; }
			set { localizableStringId = value; }
		}

		private void Start()
		{
			ui = GetComponent<Text>();
		}

		protected override void Update()
		{
			base.Update();
			ui.text = I18n[localizableStringId];
		}
	}
}