using System.Collections.Generic;
using WoF.Reward;
using Random = UnityEngine.Random;

namespace WoF.Wheel
{
	/// <summary>
	/// Rolls a rarity per slot against a table that comes from data.
	/// Adding or reordering a rarity is a table edit, not a new branch in this class.
	/// </summary>
	public class RarityCalculator
	{
		private const int MinRoll = 1;
		private const int MaxRollExclusive = 101;
		private const int FullChance = 100;

		private readonly int _slotCount;
		private readonly IReadOnlyList<RarityChance> _rarityChances;
		private readonly EItemRarity _baseRarity;

		public RarityCalculator(int slotCount, IReadOnlyList<RarityChance> rarityChances, EItemRarity baseRarity)
		{
			_slotCount = slotCount;
			_rarityChances = rarityChances;
			_baseRarity = baseRarity;
		}

		public Dictionary<EItemRarity, int> GetItemRarityCount(WheelType wheelType)
		{
			Dictionary<EItemRarity, int> rewardCountByRarity = new Dictionary<EItemRarity, int>();

			for (int i = 0; i < _slotCount; ++i)
			{
				EItemRarity currentRarity = GetRandomRarity(wheelType);

				if (!rewardCountByRarity.TryAdd(currentRarity, 1))
				{
					++rewardCountByRarity[currentRarity];
				}
			}

			return rewardCountByRarity;
		}

		public EItemRarity GetRandomRarity(WheelType wheelType)
		{
			int roll = Random.Range(MinRoll, MaxRollExclusive);
			int threshold = FullChance;

			if (_rarityChances != null)
			{
				for (int i = 0; i < _rarityChances.Count; ++i)
				{
					RarityChance rarityChance = _rarityChances[i];

					threshold -= rarityChance.Chance;

					if (roll > threshold && WheelTypeHasRarity(wheelType, rarityChance.Rarity))
					{
						return rarityChance.Rarity;
					}
				}
			}

			return _baseRarity;
		}

		public bool WheelTypeHasRarity(WheelType wheelType, EItemRarity itemRarity)
		{
			if (wheelType == null)
			{
				return false;
			}

			bool rarityExists = wheelType.WheelTypeContent.RewardByRarity.TryGetValue(itemRarity, out List<RewardDefinition> rewards);

			return rarityExists && rewards.Count > 0;
		}
	}
}
