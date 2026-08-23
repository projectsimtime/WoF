using System;
using UnityEngine;
using WoF.Advertisement;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelSeeAdButton : ButtonController
	{
		[SerializeField]
		private AdPanelController _adPanel;

		public event Action HintRewardGranted;

		protected override void OnButtonClicked()
		{
			_adPanel.Play(new AdRewardZoneHint(OnHintRewardGranted), 4.0f);
		}

		private void OnHintRewardGranted()
		{
			HintRewardGranted?.Invoke();
		}
	}
}
