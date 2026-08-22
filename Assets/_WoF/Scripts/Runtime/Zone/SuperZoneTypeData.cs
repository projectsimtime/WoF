using UnityEngine;
using WoF.Reward;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneType_Super", menuName = "WoF/Zone/New Super Zone Type", order = 2)]
	public class SuperZoneTypeData : ZoneTypeData
	{
		[SerializeField]
		private RewardDefinition[] _specialItems;

		public override int Priority => 2;

		public override bool IsValid(int zoneIndex, GameSettings gameSettings)
		{
			return zoneIndex % gameSettings.SuperZoneFrequency == 0;
		}

		public override RewardDefinition GetReservedReward(int zoneIndex, GameSettings gameSettings, int runSeed)
		{
			int rewardIndex = (zoneIndex / gameSettings.SuperZoneFrequency) - 1;
			int firstRewardIndex = runSeed % _specialItems.Length;

			return _specialItems[(firstRewardIndex + rewardIndex) % _specialItems.Length];
		}
	}
}
