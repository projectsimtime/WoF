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
		private ZoneTypeCalculator _zoneTypeCalculator;
		private RarityCalculator _rarityCalculator;
		private int _slotCount;

		private Dictionary<int, int> _zoneOverrides = new();
		private Dictionary<ZoneTypeData, int> _nextReservedRewardIndexByZoneType = new();

		public ZoneWheelBuilder(ZoneRules zoneRules, ZoneTypeCalculator zoneTypeCalculator, int slotCount)
		{
			_zoneRules = zoneRules;
			_zoneTypeCalculator = zoneTypeCalculator;
			_slotCount = slotCount;
			_rarityCalculator = new RarityCalculator(slotCount);

			for (int i = 0; i < _zoneRules.ZoneOverrides.Length; ++i)
			{
				_zoneOverrides.Add(_zoneRules.ZoneOverrides[i].ZoneNumber, i);
			}
		}

		public void Reset()
		{
			_nextReservedRewardIndexByZoneType.Clear();

			foreach (ZoneTypeData zoneTypeData in _zoneTypeCalculator.ZoneTypes)
			{
				_nextReservedRewardIndexByZoneType.Add(zoneTypeData, 0);
			}
		}

		public ZoneWheelData Build(int zoneIndex)
		{
			ZoneWheelData zoneWheelData;
			ZoneTypeData zoneTypeData = _zoneTypeCalculator.GetZoneTypeDataFromIndex(zoneIndex);
			RewardDefinition reservedReward = GetReservedReward(zoneIndex);
			MoveToNextReservedReward(zoneTypeData);

			zoneWheelData.WheelType = zoneTypeData.WheelType;

			if (_zoneOverrides.TryGetValue(zoneIndex, out int overrideIndex))
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
			ZoneTypeData zoneTypeData = _zoneTypeCalculator.GetZoneTypeDataFromIndex(zoneIndex);
			RewardDefinition[] reservedRewards = zoneTypeData.ReservedRewards;

			if (reservedRewards.Length == 0)
			{
				return null;
			}

			int rewardIndex = _nextReservedRewardIndexByZoneType[zoneTypeData];

			return reservedRewards[rewardIndex % reservedRewards.Length];
		}

		private void MoveToNextReservedReward(ZoneTypeData zoneTypeData)
		{
			if (zoneTypeData.ReservedRewards.Length == 0)
			{
				return;
			}

			++_nextReservedRewardIndexByZoneType[zoneTypeData];
		}

		private List<RewardDefinition> GetZoneContents(WheelType wheelType)
		{
			Dictionary<RarityDefinition, int> itemRarities = _rarityCalculator.GetItemRarityCount(wheelType);
			List<RewardDefinition> rewards = new List<RewardDefinition>(_slotCount);

			foreach (var item in itemRarities)
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
