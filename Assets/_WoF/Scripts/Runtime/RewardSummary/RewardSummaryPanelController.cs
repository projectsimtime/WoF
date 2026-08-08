using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using WoF.UI;

namespace WoF.RewardSummary
{
	public class RewardSummaryPanelController : PanelController
	{
		[SerializeField]
		private RewardCardView _cardViewPrefab;

		[SerializeField]
		private RectTransform _cardContent;

		[SerializeField]
		private RewardSummaryContinueButton _continueButton;

		private readonly List<RewardCardView> _cardViews = new();

		private void OnValidate()
		{
			_continueButton = GetComponentInChildren<RewardSummaryContinueButton>(true);
		}

		public void DisplayEarnedRewards(EarnedRewardContainer earnedRewardContainer)
		{
			Clear();

			foreach (var earnedReward in earnedRewardContainer.GetEarnedRewards())
			{
				RewardCardView rewardCardView = Instantiate(_cardViewPrefab, _cardContent, false);

				rewardCardView.ApplyStyle(new EarnedRewardData
				{
					Reward = earnedReward.Key,
					Amount = earnedReward.Value
				});

				_cardViews.Add(rewardCardView);
			}
		}

		public void Clear()
		{
			// TODO : Add pooling.
			foreach (RewardCardView cardView in _cardViews)
			{
				Destroy(cardView.gameObject);
			}

			_cardViews.Clear();
		}
	}
}
