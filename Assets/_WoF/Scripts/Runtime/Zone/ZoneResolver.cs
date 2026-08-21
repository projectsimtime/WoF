using System;
using System.Collections.Generic;

namespace WoF.Zone
{
	public struct ZoneOccurrence
	{
		public ZoneOccurrence(ZoneTypeData zoneType, int zoneIndex)
		{
			ZoneType = zoneType;
			ZoneIndex = zoneIndex;
		}

		public ZoneTypeData ZoneType { get; }
		public int ZoneIndex { get; }
	}

	public class ZoneResolver
	{
		private readonly List<ZoneTypeData> _zoneTypes;

		public ZoneResolver(IReadOnlyList<ZoneTypeData> zoneTypes)
		{
			if (zoneTypes == null || zoneTypes.Count == 0)
			{
				throw new ArgumentException("At least one zone type is required.", nameof(zoneTypes));
			}

			_zoneTypes = new List<ZoneTypeData>(zoneTypes.Count);

			for (int i = 0; i < zoneTypes.Count; ++i)
			{
				if (zoneTypes[i] == null)
				{
					throw new ArgumentException($"Zone type at index {i} is missing.", nameof(zoneTypes));
				}

				_zoneTypes.Add(zoneTypes[i]);
			}
		}

		public ZoneTypeData Resolve(int zoneIndex)
		{
			if (zoneIndex < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(zoneIndex), "Zone indices start at 1.");
			}

			ZoneTypeData resolvedZoneType = null;

			for (int i = 0; i < _zoneTypes.Count; ++i)
			{
				ZoneTypeData candidate = _zoneTypes[i];

				if (!candidate.Matches(zoneIndex))
				{
					continue;
				}

				if (resolvedZoneType == null || candidate.Priority > resolvedZoneType.Priority)
				{
					resolvedZoneType = candidate;
					continue;
				}

				if (candidate.Priority == resolvedZoneType.Priority)
				{
					throw new InvalidOperationException(
						$"Zone {zoneIndex} matches '{resolvedZoneType.name}' and '{candidate.name}' with the same priority.");
				}
			}

			if (resolvedZoneType == null)
			{
				throw new InvalidOperationException($"No zone type matches zone {zoneIndex}.");
			}

			return resolvedZoneType;
		}

		public IReadOnlyList<ZoneOccurrence> GetUpcomingOccurrences(int currentZoneIndex, int endGameZoneIndex)
		{
			List<ZoneOccurrence> occurrences = new List<ZoneOccurrence>();

			for (int i = 0; i < _zoneTypes.Count; ++i)
			{
				ZoneTypeData zoneType = _zoneTypes[i];

				if (!zoneType.ShowInUpcomingZoneIndicator)
				{
					continue;
				}

				int nextZoneIndex = FindNextResolvedZone(zoneType, currentZoneIndex, endGameZoneIndex);

				if (nextZoneIndex > 0)
				{
					occurrences.Add(new ZoneOccurrence(zoneType, nextZoneIndex));
				}
			}

			occurrences.Sort((left, right) =>
				left.ZoneType.UpcomingIndicatorOrder.CompareTo(right.ZoneType.UpcomingIndicatorOrder));

			return occurrences;
		}

		private int FindNextResolvedZone(ZoneTypeData zoneType, int currentZoneIndex, int endGameZoneIndex)
		{
			int firstZoneIndex = zoneType.ShouldShowCurrentZoneInIndicator(currentZoneIndex)
				? currentZoneIndex
				: currentZoneIndex + 1;

			for (int zoneIndex = firstZoneIndex; zoneIndex <= endGameZoneIndex; ++zoneIndex)
			{
				if (Resolve(zoneIndex) == zoneType)
				{
					return zoneIndex;
				}
			}

			return -1;
		}
	}
}
