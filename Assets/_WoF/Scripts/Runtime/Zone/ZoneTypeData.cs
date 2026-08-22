using UnityEngine;
using WoF.Reward;
using WoF.Wheel;
using WoF.ZoneView;

namespace WoF.Zone
{
	public abstract class ZoneTypeData : ScriptableObject
	{
		[SerializeField]
		private ZoneViewStaticData _viewData;
		[SerializeField]
		private WheelType _wheelType;

		public ZoneViewStaticData ViewData => _viewData;
		public WheelType WheelType => _wheelType;

		public abstract int Priority { get; }

		public abstract bool IsValid(int zoneIndex, GameSettings gameSettings);
		public abstract RewardDefinition GetReservedReward(int zoneIndex, GameSettings gameSettings, int runSeed);
	}
}
