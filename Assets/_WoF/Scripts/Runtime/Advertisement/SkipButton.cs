using UnityEngine;

namespace WoF.Advertisement
{
	public class SkipButton : ButtonController
	{
		[SerializeField]
		private AdPanelController _adPanel;

		protected override void OnValidate()
		{
			base.OnValidate();

			_adPanel = GetComponentInParent<AdPanelController>(true);
		}

		protected override void OnButtonClicked()
		{
			_adPanel.Skip();
		}
	}
}
