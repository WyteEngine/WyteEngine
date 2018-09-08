using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WyteEngine.I18n;

namespace WyteEngine.UI
{
	[ExecuteInEditMode]
	public class ConfigController : SingletonBaseBehaviour<ConfigController>
	{
		[SerializeField]
		Image quitButton;

		[SerializeField]
		Sprite quitIcon;

		[SerializeField]
		Sprite backIcon;

		[SerializeField]
		LocalizableUIText titleText;

		Stack<GameObject> prevView;

		[SerializeField]
		GameObject currentView;

		readonly Vector3 visibleScale = new Vector3(1, 1, 1);
		readonly Vector3 invisibleScale = new Vector3(0, 0, 1);

		[SerializeField]
		private bool isVisible;

		public bool IsVisible
		{
			get { return isVisible; }
			set { isVisible = value; }
		}

		private void Start()
		{
			prevView = new Stack<GameObject>();
			titleText.LocalizableStringId = currentView.name;
		}

		public void Navigate(GameObject view)
		{
			if (currentView != null)
			{
				currentView.SetActive(false);
				prevView.Push(currentView);
			}
			currentView = view;
			titleText.LocalizableStringId = currentView.name;
			currentView.SetActive(true);
		}

		public void Back()
		{
			if (prevView.Count == 0)
			{
				Hide();
			}
			else
			{
				if (currentView != null)
				{
					currentView.SetActive(false);
				}
				currentView = prevView.Pop();
				titleText.LocalizableStringId = currentView.name;
				currentView.SetActive(true);
			}
		}

		public void Show()
		{
			if (IsVisible) return;
			Sfx.Play("system.menu.open");
			Wyte.IsNotFreezed = false;
			IsVisible = true;
		}

		public void Hide()
		{
			if (!IsVisible) return;
			Sfx.Play("system.menu.close");
			Wyte.IsNotFreezed = true;
			IsVisible = false;
		}

		protected override void Update()
		{
			base.Update();
			quitButton.sprite = prevView?.Count > 0 ? backIcon : quitIcon;

			if (Wyte.Escape)
				Hide();

			transform.localScale = Vector3.Lerp(transform.localScale, IsVisible ? visibleScale : invisibleScale, 0.8f);
		}			
	}

}
