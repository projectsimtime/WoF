using System;
using System.Collections.Generic;
using System.Linq;
using WoF.Reward;
using WoF.Wheel;
using Random = UnityEngine.Random;

namespace WoF.Zone
{
	public class ZoneWheelBuilder
	{
		private ZoneRules _zoneRule;
		private ZoneTypeData[] _zoneTypes;
		private GameSettings _gameSettings;

		private RarityCalculator _rarityCalculator;
		private Dictionary<int, int> _zoneOverrides;
		private int _runSeed;

		public ZoneWheelBuilder(ZoneRules zoneRule, ZoneTypeData[] zoneTypes, GameSettings gameSettings)
		{
			_zoneRule = zoneRule;
			_zoneTypes = zoneTypes.OrderByDescending(zoneType => zoneType.Priority).ToArray();
			_gameSettings = gameSettings;

			_rarityCalculator = new RarityCalculator(_gameSettings.SlotCount);
			_zoneOverrides = new Dictionary<int, int>();

			for (int i = 0; i < _zoneRule.ZoneOverrides.Length; ++i)
			{
				_zoneOverrides.Add(_zoneRule.ZoneOverrides[i].ZoneNumber, i);
			}

			Reset();
		}

		public void Reset()
		{
			_runSeed = Random.Range(0, Int32.MaxValue);
		}

		public ZoneTypeData GetZoneTypeData(int zoneIndex)
		{
			foreach (ZoneTypeData zoneType in _zoneTypes)
			{
				if (zoneType.IsValid(zoneIndex, _gameSettings))
				{
					return zoneType;
				}
			}

			return null;
		}

		public ZoneWheelData BuildZoneWheel(int zoneIndex)
		{
			ZoneWheelData zoneWheel;

			ZoneTypeData zoneTypeData = GetZoneTypeData(zoneIndex);
			RewardDefinition reservedReward = zoneTypeData.GetReservedReward(zoneIndex, _gameSettings, _runSeed);

			zoneWheel.WheelType = zoneTypeData.WheelType;

			if (_zoneOverrides.TryGetValue(zoneIndex, out int index))
			{
				zoneWheel.Rewards = _zoneRule.ZoneOverrides[index].Rewards.ToList();
				zoneWheel.ReservedSlotIndex = reservedReward == null ? Int32.MaxValue : zoneWheel.Rewards.IndexOf(reservedReward);

				if (zoneWheel.ReservedSlotIndex < 0)
				{
					zoneWheel.ReservedSlotIndex = Int32.MaxValue;
				}
			}
			else
			{
				zoneWheel.ReservedSlotIndex = reservedReward == null ? Int32.MaxValue : Random.Range(0, _gameSettings.SlotCount);
				zoneWheel.Rewards = GetZoneContents(zoneWheel.WheelType);

				if (zoneWheel.ReservedSlotIndex < zoneWheel.Rewards.Count)
				{
					zoneWheel.Rewards[zoneWheel.ReservedSlotIndex] = reservedReward;
				}
			}

			return zoneWheel;
		}

		public RewardDefinition GetSuperZoneReward(int superZoneIndex)
		{
			return GetZoneTypeData(superZoneIndex).GetReservedReward(superZoneIndex, _gameSettings, _runSeed);
		}

		private List<RewardDefinition> GetZoneContents(WheelType wheelType)
		{
			var rewardsByRarity = wheelType.WheelTypeContent.RewardByRarity;

			var itemRarities = _rarityCalculator.GetItemRarityCount(wheelType);

			List<RewardDefinition> rewards = new List<RewardDefinition>(_gameSettings.SlotCount);

			foreach (var item in itemRarities)
			{
				EItemRarity rarity = item.Key;
				int count = item.Value;

				for (int i = 0; i < count; ++i)
				{
					List<RewardDefinition> currentRarityItems = rewardsByRarity[rarity];
					int rarityItemCount = currentRarityItems.Count;

					int randomIndex = Random.Range(0, rarityItemCount);

					rewards.Add(currentRarityItems[randomIndex]);
				}
			}

			return rewards;
		}
	}
}
