using System;
using UnityEngine;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zonePlacement_Periodic", menuName = "WoF/Zone Placement/Periodic", order = 1)]
	public class PeriodicZonePlacementRule : ZonePlacementRule
	{
		[Min(1)]
		[SerializeField]
		private int _frequency = 1;

		[SerializeField]
		private int[] _additionalZoneIndices = Array.Empty<int>();

		public override bool Matches(int zoneIndex)
		{
			if (zoneIndex <= 0)
			{
				return false;
			}

			for (int i = 0; i < _additionalZoneIndices.Length; ++i)
			{
				if (_additionalZoneIndices[i] == zoneIndex)
				{
					return true;
				}
			}

			return _frequency > 0 && zoneIndex % _frequency == 0;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			_frequency = Mathf.Max(1, _frequency);
		}
#endif
	}
}
