using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF.Wheel
{
	public class RarityCalculator
	{
		private int _slotCount;

		private int _legendaryChance;
		private int _epicChance;
		private int _rareChance;

		public RarityCalculator(int slotCount, int legendaryChance, int epicChance, int rareChance)
		{
			_slotCount = slotCount;

			_legendaryChance = legendaryChance;
			_epicChance = epicChance;
			_rareChance = rareChance;
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
			int randomNumber = Random.Range(1, 101);

			int legendaryThreshold = 100 - _legendaryChance;
			int epicThreshold = legendaryThreshold - _epicChance;
			int rareThreshold = epicThreshold - _rareChance;

			// Note: I know "else" keyword is redundant. However, I believe this version is more readable.
			if (randomNumber > legendaryThreshold && IsRarityExistOnWheelType(wheelType, EItemRarity.Legendary))
			{
				return EItemRarity.Legendary;
			}
			else if (randomNumber > epicThreshold && IsRarityExistOnWheelType(wheelType, EItemRarity.Epic))
			{
				return EItemRarity.Epic;
			}
			else if(randomNumber > rareThreshold && IsRarityExistOnWheelType(wheelType, EItemRarity.Rare))
			{
				return EItemRarity.Rare;
			}
			else
			{
				return EItemRarity.Casual;
			}
		}

		public bool IsRarityExistOnWheelType(WheelType wheelType, EItemRarity itemRarity)
		{
			bool isRarityExists = wheelType.WheelTypeContent.RewardByRarity.TryGetValue(itemRarity, out var rewards);

			return (isRarityExists && rewards.Count > 0);
		}
	}
}
