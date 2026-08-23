using UnityEngine;

namespace WoF.Reward
{
	[CreateAssetMenu(fileName = "reward_", menuName = "WoF/Reward/New Reward")]
	public class RewardDefinition : ScriptableObject
	{
		[SerializeField]
		private string _label;
		[SerializeField]
		private Sprite _sprite;
		[SerializeField]
		private RarityDefinition _rarity;
		[SerializeField]
		private ItemKindDefinition _kind;

		public string Label => _label;
		public Sprite Sprite => _sprite;
		public RarityDefinition Rarity => _rarity;
		public ItemKindDefinition Kind => _kind;
	}
}
