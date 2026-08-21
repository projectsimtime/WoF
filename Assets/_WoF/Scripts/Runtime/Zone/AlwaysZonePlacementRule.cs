using UnityEngine;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zonePlacement_Always", menuName = "WoF/Zone Placement/Always", order = 0)]
	public class AlwaysZonePlacementRule : ZonePlacementRule
	{
		public override bool Matches(int zoneIndex)
		{
			return zoneIndex > 0;
		}
	}
}
