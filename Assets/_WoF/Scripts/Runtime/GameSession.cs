using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using WoF.Reward;
using WoF.Zone;
using Random = UnityEngine.Random;

namespace WoF
{
	public class GameSession : MonoBehaviour
	{
		[SerializeField]
		private ZoneRules _zoneRule;

		[SerializeField]
		private Dictionary<int, int> _zoneOverrides;

		private int _currentZone = 1;

		private int reservedSlotIndex;
		
		[SerializeField] 
		private WheelType[] _wheelTypes;

		[SerializeField] 
		private WheelParent _wheelParent;

		[SerializeField]
		private RewardDefinition _bombReward;

		[SerializeField]
		private RewardDefinition[] _specialItems;

		[SerializeField] 
		private GameSettings _gameSettings;

		private RewardDefinition _earnedReward;

		private List<RewardDefinition> _rewardDefinitions;

		private bool isContinueAfterBomb;

		private void OnValidate()
		{
			_zoneOverrides = new Dictionary<int, int>();

			for (int i = 0; i < _zoneRule.ZoneOverrides.Length; ++i)
			{
				_zoneOverrides.Add(_zoneRule.ZoneOverrides[i].ZoneNumber, i);
			}
		}

		public void OnLevelStart()
		{
			_wheelParent.RestoreWheelRotation();

			BuildZoneContents();
			// if (_zoneOverrides.TryGetValue(_currentZone, out int index))
			// {
			// 	_rewardDefinitions = _zoneRule.ZoneOverrides[index].Rewards.ToList();
			// }
			// else
			// {
			// 	_rewardDefinitions = BuildZoneContents();
			// }
		}

		public void GetRandomIndexTurnCount(int reservedIndex, bool shouldSkipReservedIndex, out int slotIndex, out int turnCount)
		{
			int randomSlotIndex = Random.Range(0, _gameSettings.SlotCount);

			if (shouldSkipReservedIndex && randomSlotIndex == reservedIndex)
			{
				randomSlotIndex = (randomSlotIndex + 1) % 8;
			}

			slotIndex = randomSlotIndex;
			turnCount = Random.Range(2, 4);
		}

		public float CalculateTargetAngle(int slotIndex, bool shouldSkipReservedIndex, out int randomSlotIndex)
		{
			GetRandomIndexTurnCount(slotIndex, shouldSkipReservedIndex, out randomSlotIndex, out int turnCount);
			float angle;

			if (!shouldSkipReservedIndex && IsRandomIndexCloseToReservedSlot(randomSlotIndex))
			{
				bool shouldLookLikeShowNearHit = (randomSlotIndex + 1) % _gameSettings.SlotCount == slotIndex;

				if (shouldLookLikeShowNearHit)
				{
					Debug.Log("100");
					angle = GetAngleFromIndex(randomSlotIndex, turnCount,0.80f);
				}
				else
				{
					Debug.Log("200");
					angle = GetAngleFromIndex(randomSlotIndex, turnCount,0.20f);
				}

			}
			else if(slotIndex == randomSlotIndex)
			{
				bool shouldLookLikeShowNearMiss = GetProbability(0.5f);

				Debug.Log($"300 shouldLookLikeShowNearMiss({shouldLookLikeShowNearMiss})");
				angle = GetAngleFromIndex(randomSlotIndex, turnCount, shouldLookLikeShowNearMiss ? 0.80f : 0.20f);
			}
			else
			{
				Debug.Log("400");
				float alpha = Random.Range(0.2f, 0.8f);

				angle = GetAngleFromIndex(randomSlotIndex, turnCount, alpha);
			}
			
			return angle;
		}
		
		public TweenerCore<Quaternion, Vector3, QuaternionOptions> OnSpinClicked()
		{
			float angle = CalculateTargetAngle(reservedSlotIndex, isContinueAfterBomb, out var earnedRewardIndex);
			
			var tween = _wheelParent.PlaySpin(angle, 3.0f);
			
			tween.onComplete += OnSpinComplete;

			_earnedReward = _rewardDefinitions[earnedRewardIndex];

			return tween;
		}

		private void OnSpinComplete()
		{
			StartCoroutine(NextLevel());
		}

		IEnumerator NextLevel()
		{
			yield return new WaitForSeconds(2);
			OnLevelStart();
			++_currentZone;
		}

		private void Start()
		{
			DOTween.Init();

			OnLevelStart();
		}

		private bool GetProbability(float alpha)
		{
			return Random.value < alpha;
		}
		private float GetAngleFromIndex(int index, int turnCount, float alpha = 0.5f)
		{
			return (index * -45.0f) + (turnCount * 360.0f) + Mathf.Lerp(-20.0f, 20.0f, alpha);
		}

		private bool IsRandomIndexCloseToReservedSlot(int index)
		{
			return (index + 1 + _gameSettings.SlotCount) % _gameSettings.SlotCount == reservedSlotIndex || 
			       (index - 1 + _gameSettings.SlotCount) % _gameSettings.SlotCount == reservedSlotIndex;
		}

		private void BuildZoneContents()
		{
			WheelType currentWheelType;
			EWheelType wheelType;

			if (_currentZone % _gameSettings.SuperZoneFrequency == 0)
			{
				currentWheelType = _wheelTypes[(int)EWheelType.Gold];
				wheelType = EWheelType.Gold;
			}
			else if (_currentZone % _gameSettings.SafeZoneFrequency == 0)
			{
				currentWheelType = _wheelTypes[(int)EWheelType.Silver];
				wheelType = EWheelType.Silver;
			}
			else
			{
				currentWheelType = _wheelTypes[(int)EWheelType.Bronze];
				wheelType = EWheelType.Bronze;
			}

			_wheelParent.ApplyWheelType(currentWheelType);

			if (_zoneOverrides.TryGetValue(_currentZone, out int index))
			{
				_rewardDefinitions = _zoneRule.ZoneOverrides[index].Rewards.ToList();
				
				for (int i = 0; i < _rewardDefinitions.Count; ++i)
				{
					_wheelParent.ApplyWheelSlot(i, _rewardDefinitions[i]);
				}
			}
			else
			{
				bool hasBomb = wheelType == EWheelType.Bronze;
				bool hasSpecialItem = wheelType == EWheelType.Gold;

				reservedSlotIndex = hasBomb || hasSpecialItem ? Random.Range(0, _gameSettings.SlotCount) : Int32.MaxValue;

				var zoneItems = GetZoneContents(currentWheelType);

				for (int i = 0; i < zoneItems.Count; ++i)
				{
					_wheelParent.ApplyWheelSlot(i, zoneItems[i]);
				}

				if (reservedSlotIndex < zoneItems.Count)
				{
					RewardDefinition insertedReward = hasBomb ? _bombReward : _specialItems[0];
					_wheelParent.ApplyWheelSlot(reservedSlotIndex, insertedReward);
					zoneItems[reservedSlotIndex] = insertedReward;
				}
				
				_rewardDefinitions = zoneItems;
			}
		}

		private List<RewardDefinition> GetZoneContents(WheelType wheelType)
		{
			var rewardsByRarity = wheelType.WheelTypeContent.RewardByRarity;

			var itemRarities = GetItemRarityCount(wheelType);

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

		public Dictionary<EItemRarity, int> GetItemRarityCount(WheelType wheelType)
		{
			Dictionary<EItemRarity, int> rewardCountByRarity = new Dictionary<EItemRarity, int>();

			// -1 because we fill the remaining slot with bomb or special item.
			for (int i = 0; i < _gameSettings.SlotCount; ++i)
			{
				EItemRarity currentRarity = GetRandomRarity(wheelType);
				
				if (!rewardCountByRarity.TryAdd(currentRarity, 1))
				{
					++rewardCountByRarity[currentRarity];
				}
			}

			Debug.Log("ilker");

			return rewardCountByRarity;
		}

		public EItemRarity GetRandomRarity(WheelType wheelType)
		{
			int randomNumber = Random.Range(1, 101);

			// Note: I know "else" keyword is redundant. However, I believe this version is more readable.
			if (randomNumber > 99 && IsRarityExistOnWheelType(wheelType, EItemRarity.Legendary))
			{
				return EItemRarity.Legendary;
			}
			else if (randomNumber > 90 && IsRarityExistOnWheelType(wheelType, EItemRarity.Epic))
			{
				return EItemRarity.Epic;
			}
			else if(randomNumber > 70 && IsRarityExistOnWheelType(wheelType, EItemRarity.Rare))
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

		public int GetNextSafeZone(int currentZoneIndex, int safeZoneFrequency = 5)
		{
			if (currentZoneIndex == 1)
			{
				return 1;
			}
			
			int possibleNextSafeZoneIndex = GetNextSpecialZoneIndex(currentZoneIndex, safeZoneFrequency);

			if (possibleNextSafeZoneIndex % 30 == 0)
			{
				return GetNextSpecialZoneIndex(possibleNextSafeZoneIndex + 1, safeZoneFrequency);
			}

			return possibleNextSafeZoneIndex;
		}

		public int GetNextSuperZone(int currentZoneIndex, int superZoneFrequency = 30)
		{
			return GetNextSpecialZoneIndex(currentZoneIndex, superZoneFrequency);
		}
		
		public int GetNextSpecialZoneIndex(int currentZoneIndex, int zoneFrequency)
		{
			int possibleScaler = (currentZoneIndex / zoneFrequency) + 1;
			int possibleNextSafeZoneIndex = possibleScaler * zoneFrequency;

			return possibleNextSafeZoneIndex;
		}
	}
}