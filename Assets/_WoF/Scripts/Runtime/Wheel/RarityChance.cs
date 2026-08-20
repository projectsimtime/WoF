using System;
using UnityEngine;
using WoF.Reward;

namespace WoF.Wheel
{
	/// <summary>
	/// One row of the rarity table: how likely this rarity is to be drawn for a slot.
	/// Rows are evaluated from the top down, so the rarest rarity belongs first.
	/// </summary>
	[Serializable]
	public struct RarityChance
	{
		[SerializeField]
		private EItemRarity _rarity;
		[Range(0, 100)]
		[SerializeField]
		private int _chance;

		public EItemRarity Rarity => _rarity;
		public int Chance => _chance;
	}
}
