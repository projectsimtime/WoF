using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
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
		private GameObject _revealBackground;
		[SerializeField]
		private float _revealScale = 1.5f;
		[SerializeField]
		private float _revealScaleUpDuration = 0.25f;
		[SerializeField]
		private float _revealHoldDuration = 0.4f;
		[SerializeField]
		private float _revealScaleDownDuration = 0.2f;

		private Dictionary<RewardDefinition, RewardItemView> _rewardItemViews;
		private EarnedRewardContainer _rewardContainer;
		private RewardCardView _revealCard;

		public event Action NewRunRequested;

		public void Initialize()
		{
			_rewardItemViews = new();
			_rewardContainer = new EarnedRewardContainer();
			_revealCard = Instantiate(_revealCardPrefab, transform.root, false);
			_revealCard.gameObject.SetActive(false);
			_revealBackground.SetActive(false);

			_endGamePanelController.Initialize();
			_endGamePanelController.CollectRequested += ShowEarnedRewards;
			_rewardSummaryPanelController.Initialize();
			_rewardSummaryPanelController.ContinueRequested += OnContinueClicked;
		}

		private void OnDestroy()
		{
			_endGamePanelController.CollectRequested -= ShowEarnedRewards;
			_rewardSummaryPanelController.ContinueRequested -= OnContinueClicked;
		}

		public void ResetRun()
		{
			_revealCard.gameObject.SetActive(false);
			_revealBackground.SetActive(false);
			ClearRewardViews();
			_rewardContainer.Clear();
			_rewardSummaryPanelController.Hide();
			_endGamePanelController.Hide();
		}

		public void AddReward(RewardDefinition reward, int amount)
		{
			_rewardContainer.AddItem(reward, amount);

			EarnedRewardData earnedRewardData = new EarnedRewardData
			{
				Reward = reward, Amount = amount
			};

			bool rewardExists = _rewardItemViews.TryGetValue(reward, out var rewardItemView);
			rewardItemView ??= Instantiate(_itemViewPrefab, transform, false);

			if (!rewardExists)
			{
				_rewardItemViews.Add(reward, rewardItemView);
			}

			rewardItemView.ApplyStyle(earnedRewardData);
		}

		public IEnumerator RevealReward(RewardDefinition reward, int amount)
		{
			EarnedRewardData earnedRewardData = new EarnedRewardData
			{
				Reward = reward, Amount = amount
			};

			_revealBackground.SetActive(true);

			RectTransform revealCardTransform = _revealCard.transform as RectTransform;

			_revealCard.ApplyStyle(earnedRewardData);
			revealCardTransform.anchoredPosition = Vector2.zero;
			revealCardTransform.localScale = Vector3.zero;
			_revealCard.gameObject.SetActive(true);

			yield return revealCardTransform.DOScale(_revealScale, _revealScaleUpDuration)
				.SetEase(Ease.OutBack)
				.WaitForCompletion();

			yield return new WaitForSeconds(_revealHoldDuration);

			yield return revealCardTransform.DOScale(Vector3.zero, _revealScaleDownDuration)
				.SetEase(Ease.InBack)
				.WaitForCompletion();

			_revealCard.gameObject.SetActive(false);
			_revealBackground.SetActive(false);
			AddReward(reward, amount);
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
