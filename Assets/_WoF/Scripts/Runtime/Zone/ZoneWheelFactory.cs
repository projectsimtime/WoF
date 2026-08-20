using System.Collections.Generic;
using System.Linq;
using WoF.Reward;
using WoF.Wheel;
using Random = UnityEngine.Random;

namespace WoF.Zone
{
	/// <summary>
	/// Builds the wheel for a zone: hand authored override first, otherwise a rarity driven
	/// draw with the reserved slot (bomb or special reward) written on top.
	/// One wheel can be built ahead of time and peeked at, which is what the next zone hint uses.
	/// </summary>
	public class ZoneWheelFactory
	{
		private const int NoCachedZone = -1;

		private readonly ZoneCalculator _zoneCalculator;
		private readonly ZoneRules _zoneRules;
		private readonly RarityCalculator _rarityCalculator;
		private readonly MilestoneRewardPlanner _milestoneRewards;
		private readonly RewardDefinition _bombReward;
		private readonly int _slotCount;

		private readonly Dictionary<int, int> _overrideIndexByZone = new Dictionary<int, int>();

		private ZoneWheel _cachedWheel;
		private int _cachedZoneIndex = NoCachedZone;

		public ZoneWheelFactory(
			ZoneCalculator zoneCalculator,
			ZoneRules zoneRules,
			RarityCalculator rarityCalculator,
			MilestoneRewardPlanner milestoneRewards,
			RewardDefinition bombReward,
			int slotCount)
		{
			_zoneCalculator = zoneCalculator;
			_zoneRules = zoneRules;
			_rarityCalculator = rarityCalculator;
			_milestoneRewards = milestoneRewards;
			_bombReward = bombReward;
			_slotCount = slotCount;

			BuildOverrideIndex();
		}

		/// <summary>
		/// Returns the wheel for a zone, reusing the peeked one so the hint never lies.
		/// </summary>
		public ZoneWheel GetOrBuild(int zoneIndex)
		{
			if (_cachedZoneIndex == zoneIndex && _cachedWheel.IsValid)
			{
				ZoneWheel cached = _cachedWheel;
				InvalidateCache();

				return cached;
			}

			InvalidateCache();

			return Build(zoneIndex);
		}

		/// <summary>
		/// Builds a zone ahead of time without consuming it.
		/// </summary>
		public ZoneWheel Peek(int zoneIndex)
		{
			if (_cachedZoneIndex != zoneIndex || !_cachedWheel.IsValid)
			{
				_cachedWheel = Build(zoneIndex);
				_cachedZoneIndex = zoneIndex;
			}

			return _cachedWheel;
		}

		public void InvalidateCache()
		{
			_cachedZoneIndex = NoCachedZone;
			_cachedWheel = default;
		}

		private void BuildOverrideIndex()
		{
			if (_zoneRules == null || _zoneRules.ZoneOverrides == null)
			{
				return;
			}

			for (int i = 0; i < _zoneRules.ZoneOverrides.Length; ++i)
			{
				_overrideIndexByZone[_zoneRules.ZoneOverrides[i].ZoneNumber] = i;
			}
		}

		private ZoneWheel Build(int zoneIndex)
		{
			ZoneTypeData zoneType = _zoneCalculator.GetZoneType(zoneIndex);

			ZoneWheel zoneWheel = new ZoneWheel
			{
				WheelType = zoneType == null ? null : zoneType.WheelType,
				ReservedSlotIndex = ZoneWheel.NoReservedSlot
			};

			if (_overrideIndexByZone.TryGetValue(zoneIndex, out int overrideIndex))
			{
				zoneWheel.Rewards = _zoneRules.ZoneOverrides[overrideIndex].Rewards.ToList();
				zoneWheel.ReservedSlotIndex = FindBombSlot(zoneWheel.Rewards);

				return zoneWheel;
			}

			zoneWheel.Rewards = DrawRewards(zoneWheel.WheelType);

			if (zoneType == null || (!zoneType.HasBomb && !zoneType.HasSpecialReward))
			{
				return zoneWheel;
			}

			int reservedSlotIndex = Random.Range(0, _slotCount);

			if (reservedSlotIndex >= zoneWheel.Rewards.Count)
			{
				return zoneWheel;
			}

			RewardDefinition reservedReward = zoneType.HasBomb
				? _bombReward
				: _milestoneRewards.GetReward(_zoneCalculator.GetOrdinalOf(zoneType, zoneIndex));

			if (reservedReward == null)
			{
				return zoneWheel;
			}

			zoneWheel.Rewards[reservedSlotIndex] = reservedReward;
			zoneWheel.ReservedSlotIndex = reservedSlotIndex;

			return zoneWheel;
		}

		private static int FindBombSlot(List<RewardDefinition> rewards)
		{
			for (int i = 0; i < rewards.Count; ++i)
			{
				if (rewards[i] != null && rewards[i].IsBomb)
				{
					return i;
				}
			}

			return ZoneWheel.NoReservedSlot;
		}

		private List<RewardDefinition> DrawRewards(WheelType wheelType)
		{
			List<RewardDefinition> rewards = new List<RewardDefinition>(_slotCount);

			if (wheelType == null)
			{
				return rewards;
			}

			IReadOnlyDictionary<EItemRarity, List<RewardDefinition>> rewardsByRarity = wheelType.WheelTypeContent.RewardByRarity;

			foreach (KeyValuePair<EItemRarity, int> rarityCount in _rarityCalculator.GetItemRarityCount(wheelType))
			{
				if (!rewardsByRarity.TryGetValue(rarityCount.Key, out List<RewardDefinition> candidates) || candidates.Count == 0)
				{
					continue;
				}

				for (int i = 0; i < rarityCount.Value; ++i)
				{
					rewards.Add(candidates[Random.Range(0, candidates.Count)]);
				}
			}

			return rewards;
		}
	}
}
