using UnityEngine;

namespace WoF.ZoneView
{
	/// <summary>
	/// One upcoming milestone as the indicator bar needs it: which zone flavour it is,
	/// when it arrives, and the reward it is advertising.
	/// </summary>
	public struct ZoneMilestoneData
	{
		public ZoneViewStaticData StaticData;
		public int ZoneIndex;
		public Sprite Icon;
	}
}
