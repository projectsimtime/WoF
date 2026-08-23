using System;
using System.Collections.Generic;
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

		private Dictionary<RewardDefinition, RewardItemView> _rewardItemViews;
		private EarnedRewardContainer _rewardContainer;

		public event Action NewRunRequested;

		public void Initialize()
		{
			_rewardItemViews = new();
			_rewardContainer = new EarnedRewardContainer();

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
