using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF
{
	public class RewardPanelController : MonoBehaviour
	{
		[SerializeField]
		private RewardItemView _itemViewPrefab;

		private Dictionary<RewardDefinition, RewardItemView> _rewardItemViews;

		private void Awake()
		{
			_rewardItemViews = new();
		}

		public void AddItem(RewardDefinition reward, int amount)
		{
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

		public void Clear()
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