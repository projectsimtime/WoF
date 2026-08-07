namespace WoF.Advertisement
{
	public class AdRewardRevive : AdRewardBase
	{
		public AdRewardRevive(GameSession gameSession)
			: base(gameSession)
		{
		}

		public override void GiveAdReward()
		{
			_gameSession.OnReviveWithAd();
		}
	}
}
