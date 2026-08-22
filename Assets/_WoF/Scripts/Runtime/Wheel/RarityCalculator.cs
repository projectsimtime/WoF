using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using Random = UnityEngine.Random;

namespace WoF.Wheel
{
	public class RarityCalculator
	{
		private int _slotCount;

		public RarityCalculator(int slotCount)
		{
			_slotCount = slotCount;
		}

		public Dictionary<RarityDefinition, int> GetItemRarityCount(WheelType wheelType)
		{
			Dictionary<RarityDefinition, int> rewardCountByRarity = new Dictionary<RarityDefinition, int>();

			for (int i = 0; i < _slotCount; ++i)
			{
				RarityDefinition currentRarity = GetRandomRarity(wheelType);

				if (!rewardCountByRarity.TryAdd(currentRarity, 1))
				{
					++rewardCountByRarity[currentRarity];
				}
			}

			return rewardCountByRarity;
		}

		private RarityDefinition GetRandomRarity(WheelType wheelType)
		{
			RarityWeight[] rarityWeights = wheelType.WheelTypeContent.RarityWeights;
			int totalWeight = 0;

			foreach (RarityWeight rarityWeight in rarityWeights)
			{
				totalWeight += rarityWeight.Weight;
			}

			int randomNumber = Random.Range(0, totalWeight);

			foreach (RarityWeight rarityWeight in rarityWeights)
			{
				if (randomNumber < rarityWeight.Weight)
				{
					return rarityWeight.Rarity;
				}

				randomNumber -= rarityWeight.Weight;
			}

			return null;
		}
	}
}
