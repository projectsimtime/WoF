using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF
{
	public class EarnedRewardContainer
	{
		private Dictionary<RewardDefinition, int> _inventory = new ();
		
		public void AddItem(RewardDefinition earnedReward, int amount)
		{
			if (!_inventory.TryAdd(earnedReward, amount))
			{
				int currentItemCount = _inventory[earnedReward];

				_inventory[earnedReward] = currentItemCount + amount;
			}
		}

		public Dictionary<RewardDefinition, int> GetEarnedRewards()
		{
			return _inventory;
		}

		public void Clear()
		{
			_inventory.Clear();
		}
	}
}