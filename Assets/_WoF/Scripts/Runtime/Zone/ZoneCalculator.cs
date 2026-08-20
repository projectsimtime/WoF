using System.Collections.Generic;

namespace WoF.Zone
{
	/// <summary>
	/// Answers every "which kind of zone is this" question from the catalog.
	/// There is no per-type branching here, so adding a zone type never touches this class.
	/// </summary>
	public class ZoneCalculator
	{
		public const int FirstZoneIndex = 1;

		private readonly ZoneCatalog _zoneCatalog;
		private readonly int _endGameZoneIndex;

		public ZoneCalculator(ZoneCatalog zoneCatalog, int endGameZoneIndex)
		{
			_zoneCatalog = zoneCatalog;
			_endGameZoneIndex = endGameZoneIndex;
		}

		public int EndGameZoneIndex => _endGameZoneIndex;

		public ZoneTypeData GetZoneType(int zoneIndex)
		{
			IReadOnlyList<ZoneTypeData> zoneTypes = _zoneCatalog.ByMatchPriority;

			for (int i = 0; i < zoneTypes.Count; ++i)
			{
				if (zoneTypes[i].Matches(zoneIndex))
				{
					return zoneTypes[i];
				}
			}

			return null;
		}

		public bool CanExitAt(int zoneIndex)
		{
			ZoneTypeData zoneType = GetZoneType(zoneIndex);

			return zoneType != null && zoneType.AllowsExit;
		}

		/// <summary>
		/// The next zone that resolves to the given type after the given zone.
		/// Zones a higher priority type steals are skipped automatically.
		/// </summary>
		public int GetNextZoneIndexOf(ZoneTypeData zoneType, int fromZoneIndex)
		{
			if (zoneType == null)
			{
				return _endGameZoneIndex;
			}

			if (zoneType.MatchesFirstZone && fromZoneIndex <= FirstZoneIndex)
			{
				return FirstZoneIndex;
			}

			for (int candidate = fromZoneIndex + 1; candidate <= _endGameZoneIndex; ++candidate)
			{
				if (GetZoneType(candidate) == zoneType)
				{
					return candidate;
				}
			}

			return _endGameZoneIndex;
		}

		/// <summary>
		/// How many zones of this type a full run contains.
		/// </summary>
		public int CountZonesOf(ZoneTypeData zoneType)
		{
			if (zoneType == null)
			{
				return 0;
			}

			int count = 0;

			for (int zoneIndex = FirstZoneIndex; zoneIndex <= _endGameZoneIndex; ++zoneIndex)
			{
				if (GetZoneType(zoneIndex) == zoneType)
				{
					++count;
				}
			}

			return count;
		}

		/// <summary>
		/// Zero based position of the given zone among the zones sharing its type.
		/// </summary>
		public int GetOrdinalOf(ZoneTypeData zoneType, int zoneIndex)
		{
			if (zoneType == null)
			{
				return 0;
			}

			int ordinal = -1;

			for (int candidate = FirstZoneIndex; candidate <= zoneIndex; ++candidate)
			{
				if (GetZoneType(candidate) == zoneType)
				{
					++ordinal;
				}
			}

			return ordinal < 0 ? 0 : ordinal;
		}
	}
}
