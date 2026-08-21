using UnityEngine;

namespace WoF.Zone
{
	public abstract class ZonePlacementRule : ScriptableObject
	{
		public abstract bool Matches(int zoneIndex);
	}
}
