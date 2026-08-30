using System;
using System.Collections.Generic;
using System.Linq;
using WoF.Reward;
using WoF.Wheel;
using Random = UnityEngine.Random;

namespace WoF.Zone
{
	public struct ZoneWheelData
	{
		public WheelType WheelType;
		public List<RewardDefinition> Rewards;
		public int ReservedSlotIndex;
	}

	public class ZoneWheelBuilder
	{
		private ZoneRules _zoneRules;
		private ZoneSchedule _zoneSchedule;
		private RarityCalculator _rarityCalculator;
		private int _slotCount;

		private Dictionary<int, int> _zoneOverrideIndices = new();
		private Dictionary<ZoneDefinition, int> _nextReservedRewardIndexByZoneDefinition = new();

		public ZoneWheelBuilder(ZoneRules zoneRules, ZoneSchedule zoneSchedule, int slotCount)
		{
			_zoneRules = zoneRules;
			_zoneSchedule = zoneSchedule;
			_slotCount = slotCount;
			_rarityCalculator = new RarityCalculator(slotCount);

			for (int i = 0; i < _zoneRules.ZoneOverrides.Length; ++i)
			{
				_zoneOverrideIndices.Add(_zoneRules.ZoneOverrides[i].ZoneNumber, i);
			}
		}

		public void Reset()
		{
			_nextReservedRewardIndexByZoneDefinition.Clear();
		}

		public ZoneWheelData Build(int zoneIndex)
		{
			ZoneWheelData zoneWheelData;
			ZoneDefinition zoneDefinition = _zoneSchedule.GetZoneDefinition(zoneIndex);
			RewardDefinition reservedReward = GetReservedReward(zoneIndex);
			AdvanceReservedRewardIndex(zoneDefinition);

			zoneWheelData.WheelType = zoneDefinition.WheelType;

			if (_zoneOverrideIndices.TryGetValue(zoneIndex, out int overrideIndex))
			{
				zoneWheelData.Rewards = _zoneRules.ZoneOverrides[overrideIndex].Rewards.ToList();
				zoneWheelData.ReservedSlotIndex = reservedReward ? zoneWheelData.Rewards.IndexOf(reservedReward) : Int32.MaxValue;

				if (zoneWheelData.ReservedSlotIndex < 0)
				{
					zoneWheelData.ReservedSlotIndex = Int32.MaxValue;
				}
			}
			else
			{
				zoneWheelData.ReservedSlotIndex = reservedReward ? Random.Range(0, _slotCount) : Int32.MaxValue;
				zoneWheelData.Rewards = GetZoneContents(zoneWheelData.WheelType);

				if (zoneWheelData.ReservedSlotIndex < zoneWheelData.Rewards.Count)
				{
					zoneWheelData.Rewards[zoneWheelData.ReservedSlotIndex] = reservedReward;
				}
			}

			return zoneWheelData;
		}

		public RewardDefinition GetReservedReward(int zoneIndex)
		{
			ZoneDefinition zoneDefinition = _zoneSchedule.GetZoneDefinition(zoneIndex);
			RewardDefinition[] reservedRewards = zoneDefinition.ReservedRewards;

			if (reservedRewards.Length == 0)
			{
				return null;
			}

			_nextReservedRewardIndexByZoneDefinition.TryGetValue(zoneDefinition, out int rewardIndex);

			return reservedRewards[rewardIndex % reservedRewards.Length];
		}

		private void AdvanceReservedRewardIndex(ZoneDefinition zoneDefinition)
		{
			if (zoneDefinition.ReservedRewards.Length == 0)
			{
				return;
			}

			_nextReservedRewardIndexByZoneDefinition.TryGetValue(zoneDefinition, out int rewardIndex);
			_nextReservedRewardIndexByZoneDefinition[zoneDefinition] = rewardIndex + 1;
		}

		private List<RewardDefinition> GetZoneContents(WheelType wheelType)
		{
			Dictionary<RarityDefinition, int> rewardCountByRarity = _rarityCalculator.GetItemRarityCount(wheelType);
			List<RewardDefinition> rewards = new List<RewardDefinition>(_slotCount);

			foreach (var item in rewardCountByRarity)
			{
				RarityDefinition rarity = item.Key;
				int count = item.Value;

				for (int i = 0; i < count; ++i)
				{
					List<RewardDefinition> currentRarityItems = wheelType.GetRewardsByRarity(rarity);
					int randomIndex = Random.Range(0, currentRarityItems.Count);

					rewards.Add(currentRarityItems[randomIndex]);
				}
			}

			return rewards;
		}
	}
}
