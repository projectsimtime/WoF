using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using WoF.UI;

namespace WoF.RewardSummary
{
	public class RewardSummaryPanelController : PanelController, IRewardSummaryView
	{
		[SerializeField]
		private RewardCardView _cardViewPrefab;

		[SerializeField]
		private RectTransform _cardContent;

		[SerializeField]
		private RewardSummaryContinueButton _continueButton;

		private readonly List<RewardCardView> _cardViews = new List<RewardCardView>();

		private void OnValidate()
		{
			_continueButton = GetComponentInChildren<RewardSummaryContinueButton>(true);
		}

		public void ShowSummary(IReadOnlyDictionary<RewardDefinition, int> earnedRewards)
		{
			Show();
			Clear();

			if (earnedRewards == null)
			{
				return;
			}

			foreach (KeyValuePair<RewardDefinition, int> earnedReward in earnedRewards)
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
