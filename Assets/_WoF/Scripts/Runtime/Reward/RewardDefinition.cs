using UnityEngine;

namespace WoF.Reward
{
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
		private string _label;
		[SerializeField]
		private Sprite _sprite;
		[SerializeField]
		private RarityDefinition _rarity;
		[SerializeField]
		private EItemKind _kind;

		public string Label => _label;
		public Sprite Sprite => _sprite;
		public RarityDefinition Rarity => _rarity;
		public EItemKind Kind => _kind;
	}
}
