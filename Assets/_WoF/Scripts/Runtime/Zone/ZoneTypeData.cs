using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using WoF.Wheel;
using WoF.ZoneView;

namespace WoF.Zone
{
	/// <summary>
	/// One zone flavour, described entirely by data: how often it occurs, how it beats other
	/// flavours when several match the same zone, what it puts on the wheel and how it is shown.
	/// A new zone flavour is a new asset added to the ZoneCatalog, not a new branch in code.
	/// </summary>
	[CreateAssetMenu(fileName = "zoneType_", menuName = "WoF/Zone/New Zone Type", order = 0)]
	public class ZoneTypeData : ScriptableObject
	{
		public const int FirstZoneIndex = 1;

		[Header("Presentation")]
		[SerializeField]
		private ZoneViewStaticData _viewData;
		[SerializeField]
		private WheelType _wheelType;

		[Header("Content")]
		[SerializeField]
		private bool _hasBomb;
		[SerializeField]
		private bool _hasSpecialReward;
		[SerializeField]
		private RewardDefinition[] _specialRewardPool;

		[Header("Matching")]
		[Min(1)]
		[SerializeField]
		private int _zoneFrequency = 1;
		[Tooltip("When several zone types match the same zone, the highest priority wins.")]
		[SerializeField]
		private int _matchPriority;
		[SerializeField]
		private bool _matchesFirstZone;

		[Header("Rules")]
		[SerializeField]
		private bool _allowsExit;
		[Tooltip("Shown in the zone indicator bar as an upcoming milestone.")]
		[SerializeField]
		private bool _showsAsMilestone;

		public ZoneViewStaticData ViewData => _viewData;
		public WheelType WheelType => _wheelType;

		public bool HasBomb => _hasBomb;
		public bool HasSpecialReward => _hasSpecialReward;
		public IReadOnlyList<RewardDefinition> SpecialRewardPool => _specialRewardPool;

		public int ZoneFrequency => Mathf.Max(1, _zoneFrequency);
		public int MatchPriority => _matchPriority;
		public bool MatchesFirstZone => _matchesFirstZone;

		public bool AllowsExit => _allowsExit;
		public bool ShowsAsMilestone => _showsAsMilestone;

		public bool Matches(int zoneIndex)
		{
			if (_matchesFirstZone && zoneIndex == FirstZoneIndex)
			{
				return true;
			}

			return zoneIndex % ZoneFrequency == 0;
		}
	}
}
