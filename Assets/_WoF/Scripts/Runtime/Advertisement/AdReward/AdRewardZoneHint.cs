using System;

namespace WoF.Advertisement
{
	public class AdRewardZoneHint : AdRewardBase
	{
		private Action _onRewardGranted;

		public AdRewardZoneHint(Action onRewardGranted)
		{
			_onRewardGranted = onRewardGranted;
		}

		public override void GiveAdReward()
		{
			_onRewardGranted.Invoke();
		}
	}
}
