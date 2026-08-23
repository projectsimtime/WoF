using UnityEngine;

namespace WoF.Reward
{
	[CreateAssetMenu(fileName = "itemKind_", menuName = "WoF/Reward/New Item Kind")]
	public class ItemKindDefinition : ScriptableObject
	{
		[SerializeField]
		private bool _isExplosive;

		public bool IsExplosive => _isExplosive;
	}
}
