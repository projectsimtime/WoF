using WoF.Session;

namespace WoF.Advertisement
{
	public class ZoneHintAdReward : IAdReward
	{
		private readonly IHintCommands _hintCommands;

		public ZoneHintAdReward(IHintCommands hintCommands)
		{
			_hintCommands = hintCommands;
		}

		public void Grant()
		{
			_hintCommands.RequestHint();
		}
	}
}
