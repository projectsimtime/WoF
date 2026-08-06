namespace WoF.Zone
{
	public class ZoneCalculator
	{
		private readonly int _safeZoneFrequency;
		private readonly int _superZoneFrequency;

		public ZoneCalculator(int safeZoneFrequency, int superZoneFrequency)
		{
			_safeZoneFrequency = safeZoneFrequency;
			_superZoneFrequency = superZoneFrequency;
		}

		public bool IsSuperZone(int zoneIndex)
		{
			return zoneIndex % _superZoneFrequency == 0;
		}

		public bool IsSafeZone(int zoneIndex)
		{
			return zoneIndex % _safeZoneFrequency == 0 && !IsSuperZone(zoneIndex);
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

			return possibleNextSafeZoneIndex;
		}

		public int GetNextSuperZoneIndex(int currentZoneIndex)
		{
			return GetNextSpecialZoneIndex(currentZoneIndex, _superZoneFrequency);
		}

		public int GetNextSpecialZoneIndex(int currentZoneIndex, int zoneFrequency)
		{
			int possibleScaler = (currentZoneIndex / zoneFrequency) + 1;
			int possibleNextSafeZoneIndex = possibleScaler * zoneFrequency;

			return possibleNextSafeZoneIndex;
		}
	}
}
