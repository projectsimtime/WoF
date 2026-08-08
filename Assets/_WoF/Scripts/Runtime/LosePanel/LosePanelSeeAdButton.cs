using UnityEngine;
using WoF.Advertisement;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelSeeAdButton : ButtonController
	{
		[SerializeField]
		private AdPanelController _adPanel;

		protected override void OnValidate()
		{
			base.OnValidate();

			_adPanel = FindObjectOfType<AdPanelController>(true);
		}

		protected override void OnButtonClicked()
		{
			_adPanel.Play(new AdRewardRevive(_gameSession), 3.0f);
		}
	}
}
