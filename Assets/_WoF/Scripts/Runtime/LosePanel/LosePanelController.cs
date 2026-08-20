using UnityEngine;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelController : PanelController, ILoseView
	{
		[SerializeField]
		private LosePanelGiveUpButton _losePanelGiveUpButton;
		[SerializeField]
		private LosePanelReviveButton _losePanelReviveButton;
		[SerializeField]
		private LosePanelSeeAdButton _losePanelSeeAdButton;

		private void OnValidate()
		{
			_losePanelGiveUpButton = GetComponentInChildren<LosePanelGiveUpButton>(true);
			_losePanelReviveButton = GetComponentInChildren<LosePanelReviveButton>(true);
			_losePanelSeeAdButton = GetComponentInChildren<LosePanelSeeAdButton>(true);
		}

		public void ShowReviveCost(int amount)
		{
			_losePanelReviveButton.SetReviveCost(amount);
		}

		public void SetReviveEnabled(bool enabled)
		{
			_losePanelReviveButton.SetInteractable(enabled);
		}

		public void SetAdOfferVisible(bool visible)
		{
			_losePanelSeeAdButton.gameObject.SetActive(visible);
		}
	}
}
