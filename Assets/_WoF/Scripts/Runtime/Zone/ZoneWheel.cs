using System.Collections.Generic;
using WoF.Reward;
using WoF.Wheel;

namespace WoF.Zone
{
	/// <summary>
	/// The wheel contents a single zone will be played with.
	/// </summary>
	public struct ZoneWheel
	{
		public const int NoReservedSlot = int.MaxValue;

		public WheelType WheelType;
		public List<RewardDefinition> Rewards;
		public int ReservedSlotIndex;

		public bool IsValid => Rewards != null && WheelType != null;
	}
}
