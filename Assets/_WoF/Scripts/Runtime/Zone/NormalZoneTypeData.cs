using UnityEngine;
using WoF.Reward;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneType_Normal", menuName = "WoF/Zone/New Normal Zone Type", order = 0)]
	public class NormalZoneTypeData : ZoneTypeData
	{
		[SerializeField]
		private RewardDefinition _bombReward;

		public override int Priority => 0;

		public override bool IsValid(int zoneIndex, GameSettings gameSettings)
		{
			return true;
		}

		public override RewardDefinition GetReservedReward(int zoneIndex, GameSettings gameSettings, int runSeed)
		{
			return _bombReward;
		}
	}
}
