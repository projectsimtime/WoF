using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoF.Zone
{
	/// <summary>
	/// The full set of zone types a run can produce, ordered as they should be presented.
	/// </summary>
	[CreateAssetMenu(fileName = "zoneCatalog", menuName = "WoF/Zone/New Zone Catalog", order = 0)]
	public class ZoneCatalog : ScriptableObject
	{
		[SerializeField]
		private ZoneTypeData[] _zoneTypes;

		private List<ZoneTypeData> _byMatchPriority;

		public IReadOnlyList<ZoneTypeData> ZoneTypes => _zoneTypes ?? Array.Empty<ZoneTypeData>();

		/// <summary>
		/// Zone types from the most specific to the most generic, so the first match wins.
		/// </summary>
		public IReadOnlyList<ZoneTypeData> ByMatchPriority
		{
			get
			{
				EnsureSorted();

				return _byMatchPriority;
			}
		}

		public ZoneTypeData GetSpecialRewardZoneType()
		{
			foreach (ZoneTypeData zoneType in ZoneTypes)
			{
				if (zoneType != null && zoneType.HasSpecialReward)
				{
					return zoneType;
				}
			}

			return null;
		}

		private void OnDisable()
		{
			_byMatchPriority = null;
		}

		private void EnsureSorted()
		{
			if (_byMatchPriority != null)
			{
				return;
			}

			_byMatchPriority = new List<ZoneTypeData>();

			foreach (ZoneTypeData zoneType in ZoneTypes)
			{
				if (zoneType != null)
				{
					_byMatchPriority.Add(zoneType);
				}
			}

			_byMatchPriority.Sort((left, right) => right.MatchPriority.CompareTo(left.MatchPriority));
		}
	}
}
