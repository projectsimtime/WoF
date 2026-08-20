namespace WoF.Reward
{
	/// <summary>
	/// Owns reward bookkeeping for a run: what was earned, how much of it, and the two
	/// views that show it. Nothing here knows about zones, spins or the bomb.
	/// </summary>
	public class RewardService
	{
		private readonly EarnedRewardContainer _earnedRewards;
		private readonly IRewardAmountProvider _rewardAmounts;
		private readonly IRewardTrackView _trackView;
		private readonly IRewardSummaryView _summaryView;

		public RewardService(
			EarnedRewardContainer earnedRewards,
			IRewardAmountProvider rewardAmounts,
			IRewardTrackView trackView,
			IRewardSummaryView summaryView)
		{
			_earnedRewards = earnedRewards;
			_rewardAmounts = rewardAmounts;
			_trackView = trackView;
			_summaryView = summaryView;
		}

		public int GetAmount(RewardDefinition reward, int zoneIndex)
		{
			return _rewardAmounts.GetAmount(reward, zoneIndex);
		}

		public void Collect(RewardDefinition reward, int zoneIndex)
		{
			if (reward == null)
			{
				return;
			}

			int amount = GetAmount(reward, zoneIndex);

			_earnedRewards.AddItem(reward, amount);
			_trackView.AddReward(reward, amount);
		}

		public void ShowSummary()
		{
			_summaryView.ShowSummary(_earnedRewards.GetEarnedRewards());
		}

		public void HideSummary()
		{
			_summaryView.Hide();
		}

		public void Clear()
		{
			_earnedRewards.Clear();
			_trackView.Clear();
		}
	}
}
