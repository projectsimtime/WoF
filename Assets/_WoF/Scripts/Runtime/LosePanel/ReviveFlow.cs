using System;
using UnityEngine;
using WoF.Currency;
using WoF.UI;

namespace WoF.LosePanel
{
	public class ReviveFlow : PanelController
	{
		[SerializeField]
		private LosePanelGiveUpButton _losePanelGiveUpButton;
		[SerializeField]
		private LosePanelReviveButton _losePanelReviveButton;
		[SerializeField]
		private LosePanelSeeAdButton _losePanelSeeAdButton;

		private CurrencyContainer _currencyContainer;
		private ReviveCostCalculator _reviveCostCalculator;

		public event Action Revived;
		public event Action GaveUp;
		public event Action<int> CurrencyChanged;

		private void OnValidate()
		{
			_losePanelGiveUpButton = GetComponentInChildren<LosePanelGiveUpButton>();
			_losePanelReviveButton = GetComponentInChildren<LosePanelReviveButton>();
			_losePanelSeeAdButton = GetComponentInChildren<LosePanelSeeAdButton>();
		}

		private void OnDestroy()
		{
			_losePanelGiveUpButton.GiveUpClicked -= OnGiveUpButtonClicked;
			_losePanelReviveButton.ReviveClicked -= OnReviveWithCurrency;
			_losePanelSeeAdButton.ReviveRewardGranted -= OnReviveWithAd;
		}

		public void Initialize(CurrencyContainer currencyContainer, int initReviveCost, int reviveCostScale)
		{
			_currencyContainer = currencyContainer;
			_reviveCostCalculator = new ReviveCostCalculator(initReviveCost, reviveCostScale);

			_losePanelGiveUpButton.GiveUpClicked += OnGiveUpButtonClicked;
			_losePanelReviveButton.ReviveClicked += OnReviveWithCurrency;
			_losePanelSeeAdButton.ReviveRewardGranted += OnReviveWithAd;
		}

		public void ResetRun()
		{
			_reviveCostCalculator.Reset();
			_losePanelSeeAdButton.gameObject.SetActive(true);
			_losePanelReviveButton.SetReviveCost(_reviveCostCalculator.GetReviveCost());
		}

		public void OnBombExploded()
		{
			Show();
		}

		private void OnReviveWithCurrency()
		{
			_currencyContainer.AddCurrency(-_reviveCostCalculator.GetReviveCost());
			CompleteRevive();

			if (!_currencyContainer.HasEnoughCurrency(_reviveCostCalculator.GetReviveCost()))
			{
				_losePanelReviveButton.SetButtonInteractable(false);
			}

			CurrencyChanged?.Invoke(_currencyContainer.GetRemainingCurrencyAmount());
		}

		private void OnReviveWithAd()
		{
			_losePanelSeeAdButton.gameObject.SetActive(false);
			CompleteRevive();
		}

		private void OnGiveUpButtonClicked()
		{
			Hide();
			GaveUp?.Invoke();
		}

		private void CompleteRevive()
		{
			Hide();
			_reviveCostCalculator.OnRevived();
			_losePanelReviveButton.SetReviveCost(_reviveCostCalculator.GetReviveCost());

			Revived?.Invoke();
		}
	}
}
