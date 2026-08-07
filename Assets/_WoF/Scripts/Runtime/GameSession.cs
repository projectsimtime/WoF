using System;
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

		[SerializeField]
		private RewardAmountMap _rewardAmountMap;

		private RewardDefinition _earnedReward;

		private List<RewardDefinition> _rewardDefinitions = new();

		private bool isContinueAfterBomb;

		[SerializeField]
		private LosePanelController _losePanelController;

		[SerializeField]
		private ExitPanelController _exitPanelController;

		private EarnedRewardContainer _rewardContainer;

		[SerializeField] 
		private RewardPanelController _rewardPanelController;

		[SerializeField]
		private ZoneIndicatorPanelController _zoneIndicatorPanelController;
		
		[SerializeField]
		private CurrencyPanelController _currencyPanelController;
		[SerializeField]
		private EndGamePanelController _endGamePanelController;
		[SerializeField]
		private RewardSummaryPanelController _rewardSummaryPanelController;

		private ZoneCalculator _zoneCalculator;
		private ReviveCostCalculator _reviveCostCalculator;

		private RewardDefinition[] _superZoneRewards;

		private bool _isWheelSpinning;

		private CurrencyBag _currencyBag;
		
		

		bool CanExitNow()
		{
			return _zoneCalculator.IsSpecialZone(_currentZone) && 
			       !_isWheelSpinning;
		}


		[SerializeField] 
		private ExitButton _exitButton;

		private void Awake()
		{
			// TODO : is this okay ?
			_rewardContainer = new EarnedRewardContainer();
			_zoneCalculator = new ZoneCalculator(_gameSettings.SafeZoneFrequency, _gameSettings.SuperZoneFrequency, _gameSettings.EndGameZoneIndex);
			_reviveCostCalculator = new ReviveCostCalculator(_gameSettings.InitReviveCost, _gameSettings.ReviveCostScale);

			_currencyBag = new CurrencyBag(_gameSettings.StartCurrencyAmount);
			
			_zoneOverrides = new Dictionary<int, int>();

			for (int i = 0; i < _zoneRule.ZoneOverrides.Length; ++i)
			{
				_zoneOverrides.Add(_zoneRule.ZoneOverrides[i].ZoneNumber, i);
			}
		}

		private void CalculateSuperZoneRewards()
		{
			int superZoneCount = _gameSettings.SuperZoneCount;

			_superZoneRewards = new RewardDefinition[superZoneCount];
			
			int randomIndex = Random.Range(0, _specialItems.Length);

			for (int i = 0; i < superZoneCount; ++i)
			{
				_superZoneRewards[i] = _specialItems[randomIndex];

				randomIndex = (randomIndex + 1) % _specialItems.Length;
			}
		}

		private RewardDefinition GetSuperZoneReward(int superZoneIndex)
		{
			int rewardIndex = (superZoneIndex / _gameSettings.SuperZoneFrequency) - 1;

			return _superZoneRewards[rewardIndex];
		}

		private void OnValidate()
		{
			_rewardPanelController = FindObjectOfType<RewardPanelController>(true);
			_zoneIndicatorPanelController = FindObjectOfType<ZoneIndicatorPanelController>(true);
			_exitPanelController = FindObjectOfType<ExitPanelController>(true);
			_losePanelController = FindObjectOfType<LosePanelController>(true);
			_currencyPanelController = FindObjectOfType<CurrencyPanelController>(true);
			_endGamePanelController = FindObjectOfType<EndGamePanelController>(true);
			_rewardSummaryPanelController = FindObjectOfType<RewardSummaryPanelController>(true);
		}

		private void BuildCurrentZone()
		{
			_exitButton.SetButtonInteractable(CanExitNow());
			_wheelParent.RestoreWheelRotation();
			if (isContinueAfterBomb)
			{
				_wheelParent.SetActiveWheelSlot(reservedSlotIndex, true);
			}
			
			BuildZoneContents();

			isContinueAfterBomb = false;
		}

		private int GetRandomSlotIndex(int reservedIndex, bool shouldSkipReservedIndex)
		{
			int randomSlotIndex = Random.Range(0, _gameSettings.SlotCount);

			if (shouldSkipReservedIndex && randomSlotIndex == reservedIndex)
			{
				randomSlotIndex = (randomSlotIndex + 1) % _gameSettings.SlotCount;
			}

			return randomSlotIndex;
		}

		public float CalculateTargetAngle(int slotIndex, bool shouldSkipReservedIndex, out int randomSlotIndex)
		{
			randomSlotIndex = GetRandomSlotIndex(slotIndex, shouldSkipReservedIndex);
			float angle;

			float edgeBias = _gameSettings.SpinEdgeBias;
			float innerBias = 1.0f - edgeBias;

			if (!shouldSkipReservedIndex && IsRandomIndexCloseToReservedSlot(randomSlotIndex))
			{
				bool shouldLookLikeShowNearHit = (randomSlotIndex + 1) % _gameSettings.SlotCount == slotIndex;

				if (shouldLookLikeShowNearHit)
				{
					angle = GetAngleFromIndex(randomSlotIndex, edgeBias);
				}
				else
				{
					angle = GetAngleFromIndex(randomSlotIndex, innerBias);
				}

			}
			else if(slotIndex == randomSlotIndex)
			{
				bool shouldLookLikeShowNearMiss = GetProbability(_gameSettings.SpinNearMissChance);
				
				angle = GetAngleFromIndex(randomSlotIndex, shouldLookLikeShowNearMiss ? edgeBias : innerBias);
			}
			else
			{
				float alpha = Random.Range(innerBias, edgeBias);

				angle = GetAngleFromIndex(randomSlotIndex, alpha);
			}
			
			return angle;
		}

		public Tween OnSpinClicked()
		{
			_isWheelSpinning = true;
			_exitButton.SetButtonInteractable(CanExitNow());

			float angle = CalculateTargetAngle(reservedSlotIndex, isContinueAfterBomb, out var earnedRewardIndex);

			float duration = Random.Range(_gameSettings.SpinMinDuration, _gameSettings.SpinMaxDuration);

			var tween = _wheelParent.PlaySpin(angle, duration);
			
			tween.onComplete += OnSpinComplete;

			_earnedReward = _rewardDefinitions[earnedRewardIndex];

			return tween;
		}

		public bool IsGameFinished()
		{
			return _currentZone > _gameSettings.EndGameZoneIndex;
		}

		public void DisplayEarnedRewards()
		{
			_rewardSummaryPanelController.Show();
			_rewardSummaryPanelController.DisplayEarnedRewards(_rewardContainer);
		}

		private void OnGameFinished()
		{
			_endGamePanelController.Show();
		}
		
		private void OnSpinComplete()
		{
			_isWheelSpinning = false;
			_exitButton.SetButtonInteractable(CanExitNow());

			if (IsBomb(_earnedReward))
			{
				OnBombExploded();
			}
			else
			{
				int earnedAmount = GetRewardAmount(_earnedReward);

				_rewardContainer.AddItem(_earnedReward, earnedAmount);
				_rewardPanelController.AddItem(_earnedReward, earnedAmount);

				GoToNextZone();
			}
		}

		public bool IsBomb(RewardDefinition reward)
		{
			return reward == _bombReward;
		}

		public int GetRewardAmount(RewardDefinition reward)
		{
			return _rewardAmountMap.GetAmountByKind(reward, _currentZone);
		}

		public void OnBombExploded()
		{
			_losePanelController.Show();
		}

		private void Revive()
		{
			_losePanelController.Hide();
			isContinueAfterBomb = true;

			_wheelParent.SetActiveWheelSlot(reservedSlotIndex, false);
		}

		public void OnReviveWithCurrency()
		{
			Revive();

			_currencyBag.AddCurrency(-_reviveCostCalculator.GetReviveCost());
			_reviveCostCalculator.OnRevived();

			int nextReviveCost = _reviveCostCalculator.GetReviveCost();
			_losePanelController.UpdateReviveCost(nextReviveCost);

			if (!_currencyBag.HasEnoughCurrency(nextReviveCost))
			{
				_losePanelController.OnNotEnoughCurrencyToRevive();
			}

			_currencyPanelController.SetCurrencyAmount(_currencyBag.GetRemainingCurrencyAmount());
		}

		public void OnReviveWithAd()
		{
			_losePanelController.HideAdButton();

			Revive();
		}

		public void OnHintClicked()
		{
			Debug.Log("OnHintClicked");
		}
		
		public void OnGiveUpButtonClicked()
		{
			_losePanelController.Hide();
			StartNewRun();
		}

		public void OnExitClicked()
		{
			_exitPanelController.Show();
		}

		public void OnGoBackClicked()
		{
			_exitPanelController.Hide();
		}

		public void OnCollectRewardClicked()
		{
			_exitPanelController.Hide();
			DisplayEarnedRewards();
		}

		private void EnterZone()
		{
			BuildCurrentZone();
			RefreshZoneIndicators();
		}

		public void StartNewRun()
		{
			ClearRunState();
			EnterZone();
		}

		private void ClearRunState()
		{
			_currentZone = 1;
			_earnedReward = null;
			_exitButton.SetButtonInteractable(CanExitNow());

			_rewardDefinitions.Clear();
			_rewardPanelController.Clear();
			_rewardContainer.Clear();
			_currencyBag.Reset(_gameSettings.StartCurrencyAmount);

			_reviveCostCalculator.Reset();
			_losePanelController.UpdateReviveCost(_reviveCostCalculator.GetReviveCost());
			_currencyPanelController.SetCurrencyAmount(_currencyBag.GetRemainingCurrencyAmount());
			CalculateSuperZoneRewards();

			_rewardSummaryPanelController.Hide();
			_endGamePanelController.Hide();
		}

		private void GoToNextZone()
		{
			++_currentZone;

			if (IsGameFinished())
			{
				OnGameFinished();
				return;
			}

			EnterZone();
		}

		private void Start()
		{
			DOTween.Init();

			StartNewRun();
		}

		private void RefreshZoneIndicators()
		{
			int nextSuperZoneIndex = _zoneCalculator.GetNextSuperZoneIndex(_currentZone);

			_zoneIndicatorPanelController.OnNewZone(
				_zoneCalculator.GetNextSafeZoneIndex(_currentZone),
				nextSuperZoneIndex,
				GetSuperZoneReward(nextSuperZoneIndex).Sprite);
		}

		private bool GetProbability(float alpha)
		{
			return Random.value < alpha;
		}
		private float GetAngleFromIndex(int index, float alpha = 0.5f)
		{
			float slotPosition = index * _gameSettings.SlotAngle;
			float slotOffset = _gameSettings.SpinSlotOffsetAngle;

			int turnCount = Mathf.RoundToInt((_gameSettings.SpinTargetAngle + slotPosition) / 360.0f);

			// Its -slotPosition because slots are positioned CCW.
			return (turnCount * 360.0f) - slotPosition + Mathf.Lerp(-slotOffset, slotOffset, alpha);
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

			if (_zoneCalculator.IsSuperZone(_currentZone))
			{
				currentWheelType = _wheelTypes[(int)EWheelType.Gold];
				wheelType = EWheelType.Gold;
			}
			else if (_zoneCalculator.IsSafeZone(_currentZone))
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
					_wheelParent.ApplyWheelSlot(i, _rewardDefinitions[i], GetRewardAmount(_rewardDefinitions[i]));
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
					_wheelParent.ApplyWheelSlot(i, zoneItems[i], GetRewardAmount(zoneItems[i]));
				}

				if (reservedSlotIndex < zoneItems.Count)
				{
					RewardDefinition insertedReward = hasBomb ? _bombReward : GetSuperZoneReward(_currentZone);
					_wheelParent.ApplyWheelSlot(reservedSlotIndex, insertedReward, GetRewardAmount(insertedReward));
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
	}
}