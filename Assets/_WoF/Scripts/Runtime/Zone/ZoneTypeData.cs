using UnityEngine;
using WoF.Reward;
using WoF.Wheel;
using WoF.ZoneView;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneType_", menuName = "WoF/Zone/New Zone Type", order = 0)]
	public class ZoneTypeData : ScriptableObject
	{
		[SerializeField]
		private ZoneViewStaticData _viewData;
		[SerializeField]
		private WheelType _wheelType;
		[SerializeField]
		private int _frequency = 1;
		[SerializeField]
		private int[] _overrideIndices;
		[SerializeField]
		private bool _canExit;
		[SerializeField]
		private RewardDefinition[] _reservedRewards;

		public ZoneViewStaticData ViewData => _viewData;
		public WheelType WheelType => _wheelType;
		public int Frequency => _frequency;
		public int[] OverrideIndices => _overrideIndices;
		public bool CanExit => _canExit;
		public RewardDefinition[] ReservedRewards => _reservedRewards;
	}
}
