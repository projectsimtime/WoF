using System.Collections.Generic;
using WoF.Core;

namespace WoF.Reward
{
	/// <summary>
	/// The end of run screen listing everything the player walks away with.
	/// </summary>
	public interface IRewardSummaryView : ISceneService
	{
		void ShowSummary(IReadOnlyDictionary<RewardDefinition, int> earnedRewards);

		void Hide();
	}
}
