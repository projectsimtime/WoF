namespace WoF.Advertisement
{
	public class AdRewardZoneHint : AdRewardBase
	{
		public AdRewardZoneHint(GameSession gameSession)
			: base(gameSession)
		{
		}

		public override void GiveAdReward()
		{
			_gameSession.OnHintClicked();
		}
	}
}
