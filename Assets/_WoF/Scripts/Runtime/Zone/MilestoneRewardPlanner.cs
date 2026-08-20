using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using Random = UnityEngine.Random;

namespace WoF.Zone
{
	/// <summary>
	/// Decides, once per run, which special reward each milestone zone will carry, so the
	/// zone indicator can promise it long before that zone is actually built.
	/// </summary>
	public class MilestoneRewardPlanner
	{
		private readonly IReadOnlyList<RewardDefinition> _rewardPool;

		private RewardDefinition[] _plannedRewards = Array.Empty<RewardDefinition>();

		public MilestoneRewardPlanner(IReadOnlyList<RewardDefinition> rewardPool)
		{
			_rewardPool = rewardPool;
		}

		public void Plan(int milestoneCount)
		{
			milestoneCount = Mathf.Max(0, milestoneCount);

			_plannedRewards = new RewardDefinition[milestoneCount];

			if (_rewardPool == null || _rewardPool.Count == 0)
			{
				return;
			}

			int rewardIndex = Random.Range(0, _rewardPool.Count);

			for (int i = 0; i < milestoneCount; ++i)
			{
				_plannedRewards[i] = _rewardPool[rewardIndex];

				rewardIndex = (rewardIndex + 1) % _rewardPool.Count;
			}
		}

		public RewardDefinition GetReward(int milestoneOrdinal)
		{
			if (_plannedRewards.Length == 0)
			{
				return null;
			}

			return _plannedRewards[Mathf.Clamp(milestoneOrdinal, 0, _plannedRewards.Length - 1)];
		}
	}
}
