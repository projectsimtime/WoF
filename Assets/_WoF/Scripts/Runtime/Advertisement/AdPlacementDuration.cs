using System;
using UnityEngine;

namespace WoF.Advertisement
{
	/// <summary>
	/// How long the ad for a placement runs before it can be collected.
	/// </summary>
	[Serializable]
	public struct AdPlacementDuration
	{
		[SerializeField]
		private EAdPlacement _placement;
		[Min(0.0f)]
		[SerializeField]
		private float _duration;

		public EAdPlacement Placement => _placement;
		public float Duration => _duration;
	}
}
