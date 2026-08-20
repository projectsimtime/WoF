using WoF.Session;

namespace WoF.Advertisement
{
	public class ReviveAdReward : IAdReward
	{
		private readonly IReviveCommands _reviveCommands;

		public ReviveAdReward(IReviveCommands reviveCommands)
		{
			_reviveCommands = reviveCommands;
		}

		public void Grant()
		{
			_reviveCommands.ReviveWithAd();
		}
	}
}
