using UnityEngine;
using WoF.Reward;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneType_Safe", menuName = "WoF/Zone/New Safe Zone Type", order = 1)]
	public class SafeZoneTypeData : ZoneTypeData
	{
		public override int Priority => 1;

		public override bool IsValid(int zoneIndex, GameSettings gameSettings)
		{
			return zoneIndex == 1 || zoneIndex % gameSettings.SafeZoneFrequency == 0;
		}

		public override RewardDefinition GetReservedReward(int zoneIndex, GameSettings gameSettings, int runSeed)
		{
			return null;
		}
	}
}
