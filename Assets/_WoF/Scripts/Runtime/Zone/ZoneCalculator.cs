using UnityEngine;

namespace WoF.Zone
{
	public class ZoneCalculator
	{
		private readonly int _safeZoneFrequency;
		private readonly int _superZoneFrequency;

		private readonly int _endGameIndex;
		
		public ZoneCalculator(int safeZoneFrequency, int superZoneFrequency, int endGameIndex)
		{
			_safeZoneFrequency = safeZoneFrequency;
			_superZoneFrequency = superZoneFrequency;
			_endGameIndex = endGameIndex;
		}

		public bool IsSuperZone(int zoneIndex)
		{
			return zoneIndex % _superZoneFrequency == 0;
		}

		public bool IsSafeZone(int zoneIndex)
		{
			return zoneIndex == 1 || (zoneIndex % _safeZoneFrequency == 0 && !IsSuperZone(zoneIndex));
		}

		public bool IsSpecialZone(int zoneIndex)
		{
			return IsSafeZone(zoneIndex) || IsSuperZone(zoneIndex);
		}

		public int GetNextSafeZoneIndex(int currentZoneIndex)
		{
			if (currentZoneIndex == 1)
			{
				return 1;
			}

			int possibleNextSafeZoneIndex = GetNextSpecialZoneIndex(currentZoneIndex, _safeZoneFrequency);

			if (possibleNextSafeZoneIndex % _superZoneFrequency == 0)
			{
				return GetNextSpecialZoneIndex(possibleNextSafeZoneIndex + 1, _safeZoneFrequency);
			}

			return Mathf.Min(possibleNextSafeZoneIndex, _endGameIndex);
		}

		public int GetNextSuperZoneIndex(int currentZoneIndex)
		{
			return GetNextSpecialZoneIndex(currentZoneIndex, _superZoneFrequency);
		}

		public int GetNextSpecialZoneIndex(int currentZoneIndex, int zoneFrequency)
		{
			int possibleScaler = (currentZoneIndex / zoneFrequency) + 1;
			int possibleNextSafeZoneIndex = possibleScaler * zoneFrequency;

			return Mathf.Min(possibleNextSafeZoneIndex, _endGameIndex);
		}
	}
}
