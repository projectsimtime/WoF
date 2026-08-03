using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneRule_", menuName = "WoF/Zone/New Zone Rule", order = 0)]
	public class ZoneRules : ScriptableObject
	{
		[Serializable]
		public struct ZoneOverride
		{
			[SerializeField] 
			private uint _zoneNumber;
			[SerializeField]
			private RewardDefinition[] _rewards;

			public uint ZoneNumber => _zoneNumber;
			public RewardDefinition[] Rewards => _rewards;
		}
		
		[SerializeField]
		private ZoneOverride[] _zoneOverrides;

		public ZoneOverride[] ZoneOverrides => _zoneOverrides;
	}
}