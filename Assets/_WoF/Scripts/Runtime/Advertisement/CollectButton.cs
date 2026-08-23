using TMPro;
using UnityEngine;
using WoF.UI;

namespace WoF.Advertisement
{
	public class CollectButton : ButtonController
	{
		[SerializeField]
		private AdPanelController _adPanel;
		[SerializeField]
		private TextMeshProUGUI _buttonText;

		protected override void OnValidate()
		{
			base.OnValidate();

			_adPanel = GetComponentInParent<AdPanelController>(true);
			_buttonText = GetComponentInChildren<TextMeshProUGUI>(true);
		}

		protected override void OnButtonClicked()
		{
			_adPanel.Collect();
		}

		public void SetButtonText(string text)
		{
			_buttonText.text = text;
		}
	}
}
