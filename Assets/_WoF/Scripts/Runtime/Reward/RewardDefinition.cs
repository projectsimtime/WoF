using UnityEngine;

namespace WoF.Reward
{
	public enum EItemRarity
	{
		Casual,
		Rare,
		Epic,
		Legendary
	}

	public enum EItemKind
	{
		Currency,
		Case,
		Weapon,
		Points,
		Armor,
		Additional,
		Bomb
	}
	
	[CreateAssetMenu(fileName = "reward_", menuName = "WoF/Reward/New Reward")]
	public class RewardDefinition : ScriptableObject
	{
		[SerializeField]
		private Sprite _sprite;
		[SerializeField]
		private EItemRarity _rarity;
		[SerializeField]
		private EItemKind _kind;

		public Sprite Sprite => _sprite;
		public EItemRarity Rarity => _rarity;
		public EItemKind Kind => _kind;
	}
}