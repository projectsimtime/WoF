using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WoF.Zone
{
	public class ZoneTypeCalculator
	{
		private ZoneTypeData[] _zoneTypes;
		private int _endGameIndex;
		private SortedDictionary<int, ZoneTypeData> _overriddenIndices = new();

		public ZoneTypeData[] ZoneTypes => _zoneTypes;

		public ZoneTypeCalculator(ZoneTypeData[] zoneTypes, int endGameIndex)
		{
			_zoneTypes = zoneTypes.OrderByDescending(zoneType => zoneType.Priority).ToArray();
			_endGameIndex = endGameIndex;

			foreach (ZoneTypeData zoneTypeData in _zoneTypes)
			{
				foreach (int overrideIndex in zoneTypeData.OverrideIndices)
				{
					if (!_overriddenIndices.TryAdd(overrideIndex, zoneTypeData))
					{
						Debug.LogWarning($"Zone index ({overrideIndex}) is overridden more than once. ");
					}
				}
			}
		}

		public ZoneTypeData GetZoneTypeDataFromIndex(int zoneIndex)
		{
			if (_overriddenIndices.TryGetValue(zoneIndex, out ZoneTypeData zoneTypeData))
			{
				return zoneTypeData;
			}

			foreach (ZoneTypeData currentZoneTypeData in _zoneTypes)
			{
				if (zoneIndex % currentZoneTypeData.Frequency == 0)
				{
					return currentZoneTypeData;
				}
			}

			return null;
		}

		public int GetZonesNextIndex(ZoneTypeData zoneTypeData, int currentZoneIndex)
		{
			int possibleNextIndex = GetNextPossibleIndex(zoneTypeData, currentZoneIndex);

			while (possibleNextIndex <= _endGameIndex)
			{
				if (GetZoneTypeDataFromIndex(possibleNextIndex) == zoneTypeData)
				{
					return possibleNextIndex;
				}

				possibleNextIndex = GetNextPossibleIndex(zoneTypeData, possibleNextIndex);
			}

			return Int32.MaxValue;
		}

		private int GetNextPossibleIndex(ZoneTypeData zoneTypeData, int currentZoneIndex)
		{
			int ratio = currentZoneIndex / zoneTypeData.Frequency;
			int nextFrequencyIndex = (ratio + 1) * zoneTypeData.Frequency;
			int nextOverrideIndex = GetNextOverrideIndex(zoneTypeData, currentZoneIndex);

			return Math.Min(nextFrequencyIndex, nextOverrideIndex);
		}

		private int GetNextOverrideIndex(ZoneTypeData zoneTypeData, int currentZoneIndex)
		{
			foreach (var overriddenIndex in _overriddenIndices)
			{
				if (overriddenIndex.Key > currentZoneIndex && overriddenIndex.Value == zoneTypeData)
				{
					return overriddenIndex.Key;
				}
			}

			return Int32.MaxValue;
		}
	}
}
