using UnityEngine;

namespace WoF.Reward
{
	[CreateAssetMenu(fileName = "rarity_", menuName = "WoF/Reward/New Rarity")]
	public class RarityDefinition : ScriptableObject
	{
		[Min(0)]
		[SerializeField]
		private int _rank;

		public int Rank => _rank;
	}
}
