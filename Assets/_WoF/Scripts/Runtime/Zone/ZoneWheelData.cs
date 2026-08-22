using System.Collections.Generic;
using WoF.Reward;
using WoF.Wheel;

namespace WoF.Zone
{
	public struct ZoneWheelData
	{
		public WheelType WheelType;
		public List<RewardDefinition> Rewards;
		public int ReservedSlotIndex;
	}
}
