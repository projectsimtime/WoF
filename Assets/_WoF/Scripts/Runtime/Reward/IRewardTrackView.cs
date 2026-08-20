using WoF.Core;

namespace WoF.Reward
{
	/// <summary>
	/// The running tally of rewards collected during the current run.
	/// </summary>
	public interface IRewardTrackView : ISceneService
	{
		void AddReward(RewardDefinition reward, int amount);

		void Clear();
	}
}
