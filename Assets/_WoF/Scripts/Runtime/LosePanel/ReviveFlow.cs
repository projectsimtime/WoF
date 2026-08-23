using System;
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
		private CanvasGroup _canvasGroup;
		[SerializeField]
		private RectTransform _content;

		private CurrencyContainer _currencyContainer;
		private ReviveCostCalculator _reviveCostCalculator;
		private GameSettings _gameSettings;
		private Sequence _panelSequence;

		public event Action Revived;
		public event Action GaveUp;
		public event Action DeathDecisionShown;
		public event Action<int> CurrencyChanged;

		private void OnValidate()
		{
			_losePanelGiveUpButton = GetComponentInChildren<LosePanelGiveUpButton>(true);
			_losePanelReviveButton = GetComponentInChildren<LosePanelReviveButton>(true);
			_losePanelSeeAdButton = GetComponentInChildren<LosePanelSeeAdButton>(true);
			_canvasGroup = GetComponent<CanvasGroup>();
			_content = transform.GetChild(1) as RectTransform;
		}

		private void OnDestroy()
		{
			_panelSequence?.Kill();
			_losePanelGiveUpButton.GiveUpClicked -= OnGiveUpButtonClicked;
			_losePanelReviveButton.ReviveClicked -= OnReviveWithCurrency;
			_losePanelSeeAdButton.ReviveRewardGranted -= OnReviveWithAd;
		}

		public void Initialize(CurrencyContainer currencyContainer, int initReviveCost, int reviveCostScale, GameSettings gameSettings)
		{
			_currencyContainer = currencyContainer;
			_reviveCostCalculator = new ReviveCostCalculator(initReviveCost, reviveCostScale);
			_gameSettings = gameSettings;

			_losePanelGiveUpButton.GiveUpClicked += OnGiveUpButtonClicked;
			_losePanelReviveButton.ReviveClicked += OnReviveWithCurrency;
			_losePanelSeeAdButton.ReviveRewardGranted += OnReviveWithAd;
		}

		public void ResetRun()
		{
			_panelSequence?.Kill();
			ResetPanelVisuals();
			_reviveCostCalculator.Reset();
			_losePanelSeeAdButton.gameObject.SetActive(true);
			_losePanelReviveButton.SetButtonInteractable(true);
			_losePanelReviveButton.SetReviveCost(_reviveCostCalculator.GetReviveCost());
		}

		public void OnBombExploded()
		{
			Show();
			_panelSequence?.Kill();

			_canvasGroup.alpha = 0.0f;
			_canvasGroup.interactable = false;
			_content.localScale = Vector3.one * 0.85f;

			_panelSequence = DOTween.Sequence()
				.Append(_canvasGroup.DOFade(1.0f, _gameSettings.LosePanelFadeDuration).SetEase(Ease.Linear))
				.Join(_content.DOScale(Vector3.one, _gameSettings.LosePanelFadeDuration).SetEase(Ease.OutBack))
				.OnComplete(() =>
				{
					_canvasGroup.interactable = true;
					DeathDecisionShown?.Invoke();
				});
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
			_canvasGroup.interactable = false;
			_panelSequence?.Kill();
			_panelSequence = DOTween.Sequence()
				.Append(_canvasGroup.DOFade(0.0f, _gameSettings.LosePanelFadeDuration).SetEase(Ease.OutQuad))
				.OnComplete(() =>
				{
					Hide();
					ResetPanelVisuals();
					GaveUp?.Invoke();
				});
		}

		private void CompleteRevive()
		{
			_panelSequence?.Kill();
			Hide();
			ResetPanelVisuals();
			_reviveCostCalculator.OnRevived();
			_losePanelReviveButton.SetReviveCost(_reviveCostCalculator.GetReviveCost());

			Revived?.Invoke();
		}

		private void ResetPanelVisuals()
		{
			_canvasGroup.alpha = 1.0f;
			_canvasGroup.interactable = true;
			_content.localScale = Vector3.one;
		}
	}
}
