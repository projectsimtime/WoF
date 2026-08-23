using System;

namespace WoF.Advertisement
{
	public class AdRewardRevive : AdRewardBase
	{
		private Action _onRewardGranted;

		public AdRewardRevive(Action onRewardGranted)
		{
			_onRewardGranted = onRewardGranted;
		}

		public override void GiveAdReward()
		{
			_onRewardGranted.Invoke();
		}
	}
}
