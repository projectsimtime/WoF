using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
		}

		public void OnSpinClicked()
		{
			int randomIndex = Random.Range(0, _gameSettings.SlotCount);

			int turnCount = Random.Range(2, 4);
			float angle;

			if (IsRandomIndexCloseToReservedSlot(randomIndex))
			{
				bool shouldLookLikeShowNearHit = (randomIndex + 1) % _gameSettings.SlotCount == reservedSlotIndex;

				if (shouldLookLikeShowNearHit)
				{
					Debug.Log("100");
					angle = GetAngleFromIndex(randomIndex, turnCount,0.80f);
				}
				else
				{
					Debug.Log("200");
					angle = GetAngleFromIndex(randomIndex, turnCount,0.20f);
				}

			}
			else if(reservedSlotIndex == randomIndex)
			{
				bool shouldLookLikeShowNearMiss = GetProbability(50.0f);

				Debug.Log($"300 shouldLookLikeShowNearMiss({shouldLookLikeShowNearMiss})");
				angle = GetAngleFromIndex(randomIndex, turnCount, shouldLookLikeShowNearMiss ? 0.80f : 0.20f);
			}
			else
			{
				Debug.Log("400");
				float alpha = Random.Range(0.2f, 0.8f);

				angle = GetAngleFromIndex(randomIndex, turnCount, alpha);
			}
			
			var tween = _wheelParent.PlaySpin(angle, turnCount * 0.66f);
			
			tween.onComplete += OnSpinComplete;
			
			Debug.Log(
				$"RandomIndex({randomIndex}), ReservedIndex({reservedSlotIndex}), Angle({angle}), TurnCount({turnCount}))");
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

		private bool GetProbability(float percentage)
		{
			return Random.value < percentage / 100.0f;
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

		private List<RewardDefinition> BuildZoneContents()
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

			bool hasBomb = wheelType == EWheelType.Bronze;
			bool hasSpecialItem = wheelType == EWheelType.Gold;

			reservedSlotIndex = hasBomb || hasSpecialItem ? Random.Range(0, _gameSettings.SlotCount) : Int32.MaxValue;

			var zoneItems = GetZoneContents(currentWheelType);

			for (int i = 0; i < zoneItems.Count; ++i)
			{
				if (i == reservedSlotIndex)
				{
					continue;
				}
				
				_wheelParent.ApplyWheelSlot(i, zoneItems[i]);
			}

			if (reservedSlotIndex != Int32.MaxValue)
			{
				RewardDefinition insertedReward = hasBomb ? _bombReward : _specialItems[0];
				_wheelParent.ApplyWheelSlot(reservedSlotIndex, insertedReward);
				zoneItems.Insert(reservedSlotIndex, insertedReward);
			}

			return zoneItems;
		}

		private List<RewardDefinition> GetZoneContents(WheelType wheelType)
		{
			var rewardsByRarity = wheelType.WheelTypeContent.RewardByRarity;

			var itemRarities = GetItemRarityCount();

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

		public Dictionary<EItemRarity, int> GetItemRarityCount()
		{
			Dictionary<EItemRarity, int> rewardCountByRarity = new Dictionary<EItemRarity, int>();

			// -1 because we fill the remaining slot with bomb or special item.
			for (int i = 0; i < _gameSettings.SlotCount - 1; ++i)
			{
				EItemRarity currentRarity = GetRandomRarity();
				
				if (!rewardCountByRarity.TryAdd(currentRarity, 1))
				{
					++rewardCountByRarity[currentRarity];
				}
			}

			Debug.Log("ilker");

			return rewardCountByRarity;
		}

		public EItemRarity GetRandomRarity()
		{
			int randomNumber = Random.Range(1, 101);

			// Note: i know "else" keyword is redundant. However, I believe this version is more readable.
			if (randomNumber > 99)
			{
				return EItemRarity.Legendary;
			}
			else if (randomNumber > 90)
			{
				return EItemRarity.Epic;
			}
			else if(randomNumber > 70)
			{
				return EItemRarity.Rare;
			}
			else
			{
				return EItemRarity.Casual;
			}
		}
	}
}