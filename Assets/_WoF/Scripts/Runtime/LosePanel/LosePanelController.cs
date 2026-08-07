using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace WoF
{
	public class LosePanelController : PanelController
	{
		[SerializeField] 
		private LosePanelGiveUpButton _losePanelGiveUpButton;
		[SerializeField]
		private LosePanelReviveButton _losePanelReviveButton;
		[SerializeField]
		private LosePanelSeeAdButton _losePanelSeeAdButton;

		private void OnValidate()
		{
			_losePanelGiveUpButton = GetComponentInChildren<LosePanelGiveUpButton>();
			_losePanelReviveButton = GetComponentInChildren<LosePanelReviveButton>();
			_losePanelSeeAdButton = GetComponentInChildren<LosePanelSeeAdButton>();
		}

		public void HideAdButton()
		{
			_losePanelSeeAdButton.gameObject.SetActive(false);
		}

		public void UpdateReviveCost(int amount)
		{
			_losePanelReviveButton.SetReviveCost(amount);
		}
		public void OnNotEnoughCurrencyToRevive()
		{
			_losePanelReviveButton.SetButtonInteractable(false);
		}
	}
}