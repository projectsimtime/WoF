namespace WoF.Advertisement
{
	public abstract class AdRewardBase
	{
		protected AdRewardBase(GameSession gameSession)
		{
			_gameSession = gameSession;
		}

		protected GameSession _gameSession;

		public abstract void GiveAdReward();
	}
}