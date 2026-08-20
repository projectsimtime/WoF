namespace WoF.Reward
{
	/// <summary>
	/// How much of a reward a given zone hands out.
	/// </summary>
	public interface IRewardAmountProvider
	{
		int GetAmount(RewardDefinition reward, int zoneIndex);
	}
}
