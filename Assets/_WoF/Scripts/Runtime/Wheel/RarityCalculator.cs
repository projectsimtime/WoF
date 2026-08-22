using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF.Wheel
{
	public class RarityCalculator
	{
		private int _slotCount;

		public RarityCalculator(int slotCount)
		{
			_slotCount = slotCount;
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
			RarityChance[] rarityChances = wheelType.WheelTypeContent.RarityChances;

			int totalChance = 0;

			for (int i = 0; i < rarityChances.Length; ++i)
			{
				totalChance += rarityChances[i].Chance;
			}

			int randomNumber = Random.Range(0, totalChance);

			for (int i = 0; i < rarityChances.Length; ++i)
			{
				randomNumber -= rarityChances[i].Chance;

				if (randomNumber < 0)
				{
					return rarityChances[i].Rarity;
				}
			}

			return rarityChances[rarityChances.Length - 1].Rarity;
		}
	}
}
