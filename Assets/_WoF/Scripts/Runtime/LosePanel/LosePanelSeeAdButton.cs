using System;
using UnityEngine;
using WoF.Advertisement;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelSeeAdButton : ButtonController
	{
		[SerializeField]
		private AdPanelController _adPanel;

		public event Action ReviveRewardGranted;

		protected override void OnButtonClicked()
		{
			_adPanel.Play(new AdRewardRevive(OnReviveRewardGranted), 3.0f);
		}

		private void OnReviveRewardGranted()
		{
			ReviveRewardGranted?.Invoke();
		}
	}
}
