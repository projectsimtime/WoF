using System;
using UnityEngine;

namespace WoF.Zone
{
	[Serializable]
	public struct ZonePatternRule
	{
		[SerializeField]
		private ZoneDefinition _zoneDefinition;

		[Tooltip("How often this zone appears. Minimum is 2 because frequency 1 is reserved for the default zone, which fills all unassigned indices.")]
		[Min(2)]
		[SerializeField]
		private int _frequency;

		[SerializeField]
		[Min(0)]
		private int _priority;

		[SerializeField]
		[Tooltip("Zone indices that override recurring placements. If multiple rules target the same index, the highest priority rule wins.")]
		private int[] _overrideZoneIndices;

		public ZoneDefinition ZoneDefinition => _zoneDefinition;
		public int Frequency => _frequency;
		public int Priority => _priority;
		public int[] OverrideZoneIndices => _overrideZoneIndices ?? Array.Empty<int>();
	}

	[CreateAssetMenu(fileName = "zonePattern_", menuName = "WoF/Zone/New Zone Pattern", order = 1)]
	public class ZonePattern : ScriptableObject
	{
		[SerializeField]
		private ZoneDefinition _defaultZoneDefinition;

		[SerializeField]
		private ZonePatternRule[] _zonePatternRules;

		public ZoneDefinition DefaultZoneDefinition => _defaultZoneDefinition;
		public ZonePatternRule[] ZonePatternRules => _zonePatternRules ?? Array.Empty<ZonePatternRule>();
	}
}
