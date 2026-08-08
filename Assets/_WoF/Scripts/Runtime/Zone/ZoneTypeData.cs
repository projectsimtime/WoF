using UnityEngine;

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
		private bool _hasBomb;
		[SerializeField]
		private bool _hasSpecialReward;

		public ZoneViewStaticData ViewData => _viewData;
		public WheelType WheelType => _wheelType;
		public bool HasBomb => _hasBomb;
		public bool HasSpecialReward => _hasSpecialReward;
	}
}
