using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WoF.EndGame;
using WoF.RewardSummary;

namespace WoF.Reward
{
	public class RunRewardFlow : MonoBehaviour
	{
		[SerializeField]
		private RewardItemView _itemViewPrefab;
		[SerializeField]
		private EndGamePanelController _endGamePanelController;
		[SerializeField]
		private RewardSummaryPanelController _rewardSummaryPanelController;
		[SerializeField]
		private RewardCardView _revealCardPrefab;
		[SerializeField]
		private RectTransform _revealLayer;
		[SerializeField]
		private CanvasGroup _revealBlocker;
		[SerializeField]
		private ScrollRect _rewardScrollRect;

		private Dictionary<RewardDefinition, RewardItemView> _rewardItemViews;
		private EarnedRewardContainer _rewardContainer;
		private GameSettings _gameSettings;
		private Sequence _revealSequence;
		private Tween _scrollTween;
		private RewardCardView _activeRevealCard;

		public event Action NewRunRequested;
		public event Action RewardFlightStarted;
		public event Action RevealCompleted;

		public void Initialize(GameSettings gameSettings)
		{
			_gameSettings = gameSettings;
			_rewardItemViews = new();
			_rewardContainer = new EarnedRewardContainer();
			_revealBlocker.alpha = 0.0f;
			_revealBlocker.gameObject.SetActive(false);

			_endGamePanelController.Initialize();
			_endGamePanelController.CollectRequested += ShowEarnedRewards;
			_rewardSummaryPanelController.Initialize();
			_rewardSummaryPanelController.ContinueRequested += OnContinueClicked;
		}

		private void OnDestroy()
		{
			KillReveal();
			_endGamePanelController.CollectRequested -= ShowEarnedRewards;
			_rewardSummaryPanelController.ContinueRequested -= OnContinueClicked;
		}

		public void ResetRun()
		{
			KillReveal();
			ClearRewardViews();
			_rewardContainer.Clear();
			_rewardSummaryPanelController.Hide();
			_endGamePanelController.Hide();
		}

		public void RevealReward(RewardDefinition reward, int amount)
		{
			_rewardScrollRect.StopMovement();
			_rewardContainer.AddItem(reward, amount);

			EarnedRewardData earnedRewardData = new EarnedRewardData
			{
				Reward = reward, Amount = amount
			};

			RewardItemView rewardItemView = GetOrCreateRewardView(reward, out bool rewardExists);

			if (!rewardExists)
				rewardItemView.Prepare(reward, false);

			Canvas.ForceUpdateCanvases();
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

			PlayRevealIntro(earnedRewardData, rewardItemView);
		}

		private RewardItemView GetOrCreateRewardView(RewardDefinition reward, out bool rewardExists)
		{
			rewardExists = _rewardItemViews.TryGetValue(reward, out RewardItemView rewardItemView);

			if (rewardExists)
				return rewardItemView;

			rewardItemView = Instantiate(_itemViewPrefab, transform, false);
			_rewardItemViews.Add(reward, rewardItemView);

			return rewardItemView;
		}

		private void PlayRevealIntro(EarnedRewardData earnedRewardData, RewardItemView rewardItemView)
		{
			_activeRevealCard = Instantiate(_revealCardPrefab, _revealLayer, false);
			_activeRevealCard.ApplyStyle(earnedRewardData);

			RectTransform revealCardRect = _activeRevealCard.transform as RectTransform;
			revealCardRect.anchoredPosition = Vector2.zero;

			float heroScale = _revealLayer.rect.height * _gameSettings.RevealHeroHeightRatio / revealCardRect.rect.height;
			revealCardRect.localScale = Vector3.zero;

			_revealBlocker.gameObject.SetActive(true);
			_revealBlocker.alpha = 0.0f;

			_revealSequence = DOTween.Sequence()
				.Append(_revealBlocker.DOFade(1.0f, _gameSettings.RevealBlockerFadeDuration).SetEase(Ease.Linear))
				.Join(revealCardRect.DOScale(heroScale, _gameSettings.RevealCardScaleDuration).SetEase(Ease.OutBack))
				.OnComplete(() => HoldReveal(earnedRewardData, rewardItemView, heroScale));
		}

		private void HoldReveal(EarnedRewardData earnedRewardData, RewardItemView rewardItemView, float heroScale)
		{
			float holdDuration = earnedRewardData.Reward.Rarity.RevealHoldDuration;

			if (!IsInsideViewport(rewardItemView.RectTransform))
				ScrollToReward(rewardItemView.RectTransform, holdDuration);

			_revealSequence = DOTween.Sequence()
				.AppendInterval(holdDuration)
				.OnComplete(() => FlyToReward(earnedRewardData, rewardItemView, heroScale));
		}

		private bool IsInsideViewport(RectTransform target)
		{
			Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_rewardScrollRect.viewport, target);
			Rect viewportRect = _rewardScrollRect.viewport.rect;

			return bounds.min.y >= viewportRect.yMin && bounds.max.y <= viewportRect.yMax;
		}

		private void ScrollToReward(RectTransform target, float duration)
		{
			Canvas.ForceUpdateCanvases();

			RectTransform content = _rewardScrollRect.content;
			float scrollableHeight = content.rect.height - _rewardScrollRect.viewport.rect.height;

			if (scrollableHeight <= 0.0f)
				return;

			Bounds targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(content, target);
			float targetCenterFromTop = content.rect.yMax - targetBounds.center.y;
			float targetOffset = Mathf.Clamp(targetCenterFromTop - _rewardScrollRect.viewport.rect.height * 0.5f, 0.0f, scrollableHeight);
			float targetNormalizedPosition = 1.0f - targetOffset / scrollableHeight;

			_scrollTween?.Kill();
			_scrollTween = DOTween.To(
					() => _rewardScrollRect.verticalNormalizedPosition,
					value => _rewardScrollRect.verticalNormalizedPosition = value,
					targetNormalizedPosition,
					duration)
				.SetEase(Ease.OutQuad);
		}

		private void FlyToReward(EarnedRewardData earnedRewardData, RewardItemView rewardItemView, float heroScale)
		{
			_scrollTween?.Complete();
			RewardFlightStarted?.Invoke();
			Canvas.ForceUpdateCanvases();

			RectTransform revealCardRect = _activeRevealCard.transform as RectTransform;
			float flightDuration = _gameSettings.RevealRailFlightDuration;

			_revealSequence = DOTween.Sequence()
				.Append(_revealBlocker.DOFade(0.0f, flightDuration).SetEase(Ease.Linear))
				.Join(revealCardRect.DOMove(rewardItemView.RevealTarget.position, flightDuration).SetEase(Ease.InBack))
				.Join(revealCardRect.DOScale(heroScale * 0.5f, flightDuration).SetEase(Ease.InQuad))
				.OnComplete(() => LandReward(earnedRewardData, rewardItemView));
		}

		private void LandReward(EarnedRewardData earnedRewardData, RewardItemView rewardItemView)
		{
			Destroy(_activeRevealCard.gameObject);
			_activeRevealCard = null;
			_revealBlocker.gameObject.SetActive(false);

			rewardItemView.SetVisible(true);
			rewardItemView.IncreaseAmount(earnedRewardData.Amount, _gameSettings.RevealCounterDuration);
			rewardItemView.PlayLanding(_gameSettings.RevealLandingDuration);
			RevealCompleted?.Invoke();
		}

		private void KillReveal()
		{
			_revealSequence?.Kill();
			_scrollTween?.Kill();

			if (_activeRevealCard)
				Destroy(_activeRevealCard.gameObject);

			_activeRevealCard = null;

			if (_revealBlocker)
			{
				_revealBlocker.alpha = 0.0f;
				_revealBlocker.gameObject.SetActive(false);
			}
		}

		public void OnGameFinished()
		{
			_endGamePanelController.Show();
		}

		public void ShowEarnedRewards()
		{
			_rewardSummaryPanelController.Show();
			_rewardSummaryPanelController.DisplayEarnedRewards(_rewardContainer);
		}

		private void OnContinueClicked()
		{
			NewRunRequested?.Invoke();
		}

		private void ClearRewardViews()
		{
			// TODO : Add pooling.
			foreach (var rewardView in _rewardItemViews.Values)
			{
				Destroy(rewardView.gameObject);
			}

			_rewardItemViews.Clear();
		}
	}
}
