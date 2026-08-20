using System.Collections.Generic;
using UnityEngine;

namespace WoF.Reward
{
	public class RewardPanelController : MonoBehaviour, IRewardTrackView
	{
		[SerializeField]
		private RewardItemView _itemViewPrefab;

		private readonly Dictionary<RewardDefinition, RewardItemView> _rewardItemViews = new Dictionary<RewardDefinition, RewardItemView>();

		public void AddReward(RewardDefinition reward, int amount)
		{
			if (reward == null)
			{
				return;
			}

			if (!_rewardItemViews.TryGetValue(reward, out RewardItemView rewardItemView))
			{
				rewardItemView = Instantiate(_itemViewPrefab, transform, false);

				_rewardItemViews.Add(reward, rewardItemView);
			}

			rewardItemView.ApplyStyle(new EarnedRewardData
			{
				Reward = reward,
				Amount = amount
			});
		}

		public void Clear()
		{
			// TODO : Add pooling.
			foreach (RewardItemView rewardView in _rewardItemViews.Values)
			{
				Destroy(rewardView.gameObject);
			}

			_rewardItemViews.Clear();
		}
	}
}
