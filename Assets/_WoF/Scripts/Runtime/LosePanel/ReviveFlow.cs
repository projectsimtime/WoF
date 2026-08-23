using System;
using System.Collections;
using DG.Tweening;
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
		[SerializeField]
		private BombPanelView _view;

		private CurrencyContainer _currencyContainer;
		private ReviveCostCalculator _reviveCostCalculator;
		private float _losePanelScaleDuration;
		private bool _isClosing;

		public event Action Revived;
		public event Action GaveUp;
		public event Action<int> CurrencyChanged;

		private void OnValidate()
		{
			_losePanelGiveUpButton = GetComponentInChildren<LosePanelGiveUpButton>(true);
			_losePanelReviveButton = GetComponentInChildren<LosePanelReviveButton>(true);
			_losePanelSeeAdButton = GetComponentInChildren<LosePanelSeeAdButton>(true);
			_view = GetComponentInChildren<BombPanelView>(true);
		}

		private void OnDestroy()
		{
			_losePanelGiveUpButton.GiveUpClicked -= OnGiveUpButtonClicked;
			_losePanelReviveButton.ReviveClicked -= OnReviveWithCurrency;
			_losePanelSeeAdButton.ReviveRewardGranted -= OnReviveWithAd;
		}

		public void Initialize(CurrencyContainer currencyContainer, int initReviveCost, int reviveCostScale, float losePanelScaleDuration)
		{
			_currencyContainer = currencyContainer;
			_reviveCostCalculator = new ReviveCostCalculator(initReviveCost, reviveCostScale);
			_losePanelScaleDuration = losePanelScaleDuration;

			_losePanelGiveUpButton.GiveUpClicked += OnGiveUpButtonClicked;
			_losePanelReviveButton.ReviveClicked += OnReviveWithCurrency;
			_losePanelSeeAdButton.ReviveRewardGranted += OnReviveWithAd;
		}

		public void ResetRun()
		{
			_view.ResetView();
			_isClosing = false;
			Hide();
			_reviveCostCalculator.Reset();
			_losePanelSeeAdButton.gameObject.SetActive(true);
			_losePanelReviveButton.SetButtonInteractable(true);
			_losePanelReviveButton.SetReviveCost(_reviveCostCalculator.GetReviveCost());
		}

		public void OnBombExploded()
		{
			Show();
			_view.PlayEnter(_losePanelScaleDuration);
		}

		private void OnReviveWithCurrency()
		{
			if (_isClosing)
			{
				return;
			}

			_currencyContainer.AddCurrency(-_reviveCostCalculator.GetReviveCost());
			CompleteRevive();

			CurrencyChanged?.Invoke(_currencyContainer.GetRemainingCurrencyAmount());
		}

		private void OnReviveWithAd()
		{
			if (_isClosing)
			{
				return;
			}

			_losePanelSeeAdButton.gameObject.SetActive(false);
			CompleteRevive();
		}

		private void OnGiveUpButtonClicked()
		{
			if (_isClosing)
			{
				return;
			}

			StartCoroutine(HidePanel(false));
		}

		private void CompleteRevive()
		{
			_reviveCostCalculator.OnRevived();
			_losePanelReviveButton.SetReviveCost(_reviveCostCalculator.GetReviveCost());

			if (!_currencyContainer.HasEnoughCurrency(_reviveCostCalculator.GetReviveCost()))
			{
				_losePanelReviveButton.SetButtonInteractable(false);
			}

			StartCoroutine(HidePanel(true));
		}

		private IEnumerator HidePanel(bool revived)
		{
			_isClosing = true;

			yield return _view.PlayExit(_losePanelScaleDuration).WaitForCompletion();

			Hide();
			_isClosing = false;

			if (revived)
			{
				Revived?.Invoke();
			}
			else
			{
				GaveUp?.Invoke();
			}
		}
	}
}
