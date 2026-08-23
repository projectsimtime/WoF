using UnityEngine;

namespace WoF.Reward
{
	[CreateAssetMenu(fileName = "rarity_", menuName = "WoF/Reward/New Rarity")]
	public class RarityDefinition : ScriptableObject
	{
		[Min(0)]
		[SerializeField]
		private int _rank;
		[Min(0.0f)]
		[SerializeField]
		private float _revealHoldDuration;

		public int Rank => _rank;
		public float RevealHoldDuration => _revealHoldDuration;
	}
}
