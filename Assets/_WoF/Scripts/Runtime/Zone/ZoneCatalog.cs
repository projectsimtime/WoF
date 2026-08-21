using System.Collections.Generic;
using UnityEngine;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneCatalog", menuName = "WoF/Zone/New Zone Catalog", order = 1)]
	public class ZoneCatalog : ScriptableObject
	{
		[SerializeField]
		private ZoneTypeData[] _zoneTypes;

		public IReadOnlyList<ZoneTypeData> ZoneTypes => _zoneTypes;
	}
}
