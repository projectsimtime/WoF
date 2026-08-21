using UnityEngine;
using WoF.Wheel;
using WoF.ZoneView;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneType_", menuName = "WoF/Zone/New Zone Type", order = 0)]
	public class ZoneTypeData : ScriptableObject
	{
		[Header("Placement")]
		[SerializeField]
		private ZonePlacementRule _placementRule;
		[SerializeField]
		private int _priority;

		[Header("Gameplay")]
		[SerializeField]
		private ZoneViewStaticData _viewData;
		[SerializeField]
		private WheelType _wheelType;
		[SerializeField]
		private bool _hasBomb;
		[SerializeField]
		private bool _hasSpecialReward;
		[SerializeField]
		private bool _canExit;

		[Header("Upcoming Zone Indicator")]
		[SerializeField]
		private bool _showInUpcomingZoneIndicator;
		[SerializeField]
		private int _upcomingIndicatorOrder;
		[SerializeField]
		private int[] _showCurrentZoneIndicesInIndicator;

		public int Priority => _priority;
		public ZoneViewStaticData ViewData => _viewData;
		public WheelType WheelType => _wheelType;
		public bool HasBomb => _hasBomb;
		public bool HasSpecialReward => _hasSpecialReward;
		public bool CanExit => _canExit;
		public bool ShowInUpcomingZoneIndicator => _showInUpcomingZoneIndicator;
		public int UpcomingIndicatorOrder => _upcomingIndicatorOrder;

		public bool Matches(int zoneIndex)
		{
			if (_placementRule == null)
			{
				throw new System.InvalidOperationException($"Zone type '{name}' has no placement rule.");
			}

			return _placementRule.Matches(zoneIndex);
		}

		public bool ShouldShowCurrentZoneInIndicator(int zoneIndex)
		{
			if (_showCurrentZoneIndicesInIndicator == null)
			{
				return false;
			}

			for (int i = 0; i < _showCurrentZoneIndicesInIndicator.Length; ++i)
			{
				if (_showCurrentZoneIndicesInIndicator[i] == zoneIndex)
				{
					return true;
				}
			}

			return false;
		}
	}
}
