using UnityEngine;

namespace WyteEngine.I18n
{
	/// <summary>
	/// ローカライズ可能なSpriteです。
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class LocalizableSprite : BaseBehaviour
	{
		SpriteRenderer spriteRenderer;

		[SerializeField]
		private string path;

		/// <summary>
		/// 対象のファイルパス。
		/// </summary>
		public string Path
		{
			get { return path; }
			set { path = value; }
		}

		private void Start()
		{
			I18n.LanguageChanged += (o, n) => 
			{
				Debug.Log($"{o} {n}");
				if (o != n)
				{
					UpdateSprite();
				}
			};

			spriteRenderer = GetComponent<SpriteRenderer>();
			
			UpdateSprite();
		}

		private void UpdateSprite()
		{
			spriteRenderer.sprite = I18n.GetResource<Sprite>(Path);
		}
	}
}