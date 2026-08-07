using UnityEngine;
using WoF.Advertisement;

namespace WoF
{
	public class ExitPanelSeeAdButton : ButtonController
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
			_adPanel.Play(new AdRewardZoneHint(_gameSession), 4.0f);
		}
	}
}
