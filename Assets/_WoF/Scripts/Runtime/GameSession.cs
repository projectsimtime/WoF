using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using WoF.Currency;
using WoF.EndGame;
using WoF.ExitPanel;
using WoF.HintPanel;
using WoF.LosePanel;
using WoF.Reward;
using WoF.RewardSummary;
using WoF.Wheel;
using WoF.Zone;
using WoF.ZoneView;
using Random = UnityEngine.Random;

namespace WoF
{
	public class GameSession : MonoBehaviour
	{
		private struct ZoneWheel
		{
			public WheelType WheelType;
			public List<RewardDefinition> Rewards;
			public int ReservedSlotIndex;
		}

		[SerializeField]
		private ZoneRules _zoneRule;

		[SerializeField]
		private ZoneTypeData _normalZoneTypeData;

		[SerializeField]
		private ZoneTypeData _safeZoneTypeData;

		[SerializeField]
		private ZoneTypeData _superZoneTypeData;

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

		[SerializeField]
		private LosePanelController _losePanelController;

		[SerializeField]
		private ExitPanelController _exitPanelController;

		[SerializeField]
		private RewardPanelController _rewardPanelController;

		[SerializeField]
		private ZoneIndicatorPanelController _zoneIndicatorPanelController;

		[SerializeField]
		private CurrentZonePanelController _currentZonePanelController;

		[SerializeField]
		private CurrencyPanelController _currencyPanelController;

		[SerializeField]
		private EndGamePanelController _endGamePanelController;

		[SerializeField]
		private RewardSummaryPanelController _rewardSummaryPanelController;

		[SerializeField]
		private HintPanelController _hintPanelController;

		private Dictionary<int, int> _zoneOverrides;

		private ZoneCalculator _zoneCalculator;
		private ReviveCostCalculator _reviveCostCalculator;
		private SpinAngleCalculator _spinAngleCalculator;
		private RarityCalculator _rarityCalculator;

		private EarnedRewardContainer _rewardContainer;
		private CurrencyContainer _currencyContainer;

		private List<RewardDefinition> _rewardDefinitions = new();
		private RewardDefinition[] _superZoneRewards;
		private RewardDefinition _earnedReward;

		private ZoneWheel _nextZoneWheel;
		private bool _hasNextZoneWheel;

		private int _currentZone;
		private int _reservedSlotIndex;

		private bool _isContinueAfterBomb;
		private bool _isWheelSpinning;

		public event Action<bool> WheelSpinningChanged;
		public event Action<bool> ExitAvailabilityChanged;

		private void OnValidate()
		{
			_rewardPanelController = FindObjectOfType<RewardPanelController>(true);
			_zoneIndicatorPanelController = FindObjectOfType<ZoneIndicatorPanelController>(true);
			_currentZonePanelController = FindObjectOfType<CurrentZonePanelController>(true);
			_exitPanelController = FindObjectOfType<ExitPanelController>(true);
			_losePanelController = FindObjectOfType<LosePanelController>(true);
			_currencyPanelController = FindObjectOfType<CurrencyPanelController>(true);
			_endGamePanelController = FindObjectOfType<EndGamePanelController>(true);
			_rewardSummaryPanelController = FindObjectOfType<RewardSummaryPanelController>(true);
			_hintPanelController = FindObjectOfType<HintPanelController>(true);
		}

		private void Awake()
		{
			_rewardContainer = new EarnedRewardContainer();
			_zoneCalculator = new ZoneCalculator(_gameSettings.SafeZoneFrequency, _gameSettings.SuperZoneFrequency, _gameSettings.EndGameZoneIndex);
			_reviveCostCalculator = new ReviveCostCalculator(_gameSettings.InitReviveCost, _gameSettings.ReviveCostScale);
			_spinAngleCalculator = new SpinAngleCalculator(_gameSettings.SlotCount, _gameSettings.SpinTargetAngle, _gameSettings.SpinSlotOffsetAngle, _gameSettings.SpinEdgeBias, _gameSettings.SpinNearMissChance);
			_rarityCalculator = new RarityCalculator(_gameSettings.SlotCount, _gameSettings.LegendaryChance, _gameSettings.EpicChance, _gameSettings.RareChance);

			_currencyContainer = new CurrencyContainer(_gameSettings.StartCurrencyAmount);

			_zoneOverrides = new Dictionary<int, int>();

			for (int i = 0; i < _zoneRule.ZoneOverrides.Length; ++i)
			{
				_zoneOverrides.Add(_zoneRule.ZoneOverrides[i].ZoneNumber, i);
			}
		}

		private void Start()
		{
			DOTween.Init();

			StartNewRun();
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
			RefreshExitAvailability();

			_rewardDefinitions.Clear();
			_rewardPanelController.Clear();
			_rewardContainer.Clear();
			_currencyContainer.Reset(_gameSettings.StartCurrencyAmount);

			_reviveCostCalculator.Reset();
			_losePanelController.ShowAdButton();
			_losePanelController.UpdateReviveCost(_reviveCostCalculator.GetReviveCost());
			_currencyPanelController.SetCurrencyAmount(_currencyContainer.GetRemainingCurrencyAmount());
			CalculateSuperZoneRewards();

			_rewardSummaryPanelController.Hide();
			_endGamePanelController.Hide();
			_hintPanelController.Hide();

			_hasNextZoneWheel = false;
		}

		private void EnterZone()
		{
			BuildCurrentZone();
			RefreshZoneIndicators();
		}

		private void BuildCurrentZone()
		{
			RefreshExitAvailability();
			_wheelParent.RestoreWheelRotation();
			if (_isContinueAfterBomb)
			{
				_wheelParent.SetActiveWheelSlot(_reservedSlotIndex, true);
			}

			ZoneWheel zoneWheel = GetZoneWheel(_currentZone);

			_reservedSlotIndex = zoneWheel.ReservedSlotIndex;
			_rewardDefinitions = zoneWheel.Rewards;

			ApplyZoneWheel(zoneWheel, _currentZone);

			_isContinueAfterBomb = false;
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

		public bool IsGameFinished()
		{
			return _currentZone > _gameSettings.EndGameZoneIndex;
		}

		private void OnGameFinished()
		{
			_endGamePanelController.Show();
		}

		private void RefreshZoneIndicators()
		{
			int nextSuperZoneIndex = _zoneCalculator.GetNextSuperZoneIndex(_currentZone);

			_currentZonePanelController.SetZoneIndex(_currentZone, GetZoneTypeData(_currentZone).ViewData.ThemeColor);

			_zoneIndicatorPanelController.OnNewZone(
				_zoneCalculator.GetNextSafeZoneIndex(_currentZone),
				nextSuperZoneIndex,
				GetSuperZoneReward(nextSuperZoneIndex).Sprite);
		}

		private ZoneTypeData GetZoneTypeData(int zoneIndex)
		{
			EZoneType zoneType = _zoneCalculator.GetZoneType(zoneIndex);

			if (zoneType == EZoneType.Super)
			{
				return _superZoneTypeData;
			}

			if (zoneType == EZoneType.Safe)
			{
				return _safeZoneTypeData;
			}

			return _normalZoneTypeData;
		}

		private ZoneWheel GetZoneWheel(int zoneIndex)
		{
			if (_hasNextZoneWheel)
			{
				_hasNextZoneWheel = false;

				return _nextZoneWheel;
			}

			return BuildZoneWheel(zoneIndex);
		}

		private ZoneWheel BuildZoneWheel(int zoneIndex)
		{
			ZoneWheel zoneWheel;

			ZoneTypeData zoneTypeData = GetZoneTypeData(zoneIndex);

			zoneWheel.WheelType = zoneTypeData.WheelType;

			if (_zoneOverrides.TryGetValue(zoneIndex, out int index))
			{
				zoneWheel.Rewards = _zoneRule.ZoneOverrides[index].Rewards.ToList();
				zoneWheel.ReservedSlotIndex = zoneWheel.Rewards.IndexOf(_bombReward);

				if (zoneWheel.ReservedSlotIndex < 0)
				{
					zoneWheel.ReservedSlotIndex = Int32.MaxValue;
				}
			}
			else
			{
				bool hasBomb = zoneTypeData.HasBomb;
				bool hasSpecialItem = zoneTypeData.HasSpecialReward;

				zoneWheel.ReservedSlotIndex = hasBomb || hasSpecialItem ? Random.Range(0, _gameSettings.SlotCount) : Int32.MaxValue;
				zoneWheel.Rewards = GetZoneContents(zoneWheel.WheelType);

				if (zoneWheel.ReservedSlotIndex < zoneWheel.Rewards.Count)
				{
					zoneWheel.Rewards[zoneWheel.ReservedSlotIndex] = hasBomb ? _bombReward : GetSuperZoneReward(zoneIndex);
				}
			}

			return zoneWheel;
		}

		private void ApplyZoneWheel(ZoneWheel zoneWheel, int zoneIndex)
		{
			_wheelParent.ApplyWheelType(zoneWheel.WheelType);

			for (int i = 0; i < zoneWheel.Rewards.Count; ++i)
			{
				_wheelParent.ApplyWheelSlot(i, zoneWheel.Rewards[i], GetRewardAmount(zoneWheel.Rewards[i], zoneIndex));
			}
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

		public void OnSpinClicked()
		{
			SetWheelSpinning(true);

			float angle = _spinAngleCalculator.CalculateTargetAngle(_reservedSlotIndex, _isContinueAfterBomb, out var earnedRewardIndex);

			float duration = Random.Range(_gameSettings.SpinMinDuration, _gameSettings.SpinMaxDuration);

			var tween = _wheelParent.PlaySpin(angle, duration);

			tween.onComplete += OnSpinComplete;

			_earnedReward = _rewardDefinitions[earnedRewardIndex];
		}

		private void SetWheelSpinning(bool isSpinning)
		{
			_isWheelSpinning = isSpinning;

			WheelSpinningChanged?.Invoke(isSpinning);
			RefreshExitAvailability();
		}

		private void OnSpinComplete()
		{
			SetWheelSpinning(false);

			if (IsBomb(_earnedReward))
			{
				OnBombExploded();
			}
			else
			{
				int earnedAmount = GetRewardAmount(_earnedReward, _currentZone);

				_rewardContainer.AddItem(_earnedReward, earnedAmount);
				_rewardPanelController.AddItem(_earnedReward, earnedAmount);

				GoToNextZone();
			}
		}

		public bool IsBomb(RewardDefinition reward)
		{
			return reward == _bombReward;
		}

		public int GetRewardAmount(RewardDefinition reward, int zoneIndex)
		{
			return _rewardAmountMap.GetAmountByKind(reward, zoneIndex);
		}

		public void OnBombExploded()
		{
			_losePanelController.Show();
		}

		private void Revive()
		{
			_losePanelController.Hide();
			_isContinueAfterBomb = true;

			_wheelParent.SetActiveWheelSlot(_reservedSlotIndex, false);
		}

		public void OnReviveWithCurrency()
		{
			Revive();

			_currencyContainer.AddCurrency(-_reviveCostCalculator.GetReviveCost());
			_reviveCostCalculator.OnRevived();

			int nextReviveCost = _reviveCostCalculator.GetReviveCost();
			_losePanelController.UpdateReviveCost(nextReviveCost);

			if (!_currencyContainer.HasEnoughCurrency(nextReviveCost))
			{
				_losePanelController.OnNotEnoughCurrencyToRevive();
			}

			_currencyPanelController.SetCurrencyAmount(_currencyContainer.GetRemainingCurrencyAmount());
		}

		public void OnReviveWithAd()
		{
			_losePanelController.HideAdButton();

			Revive();
		}

		public void OnGiveUpButtonClicked()
		{
			_losePanelController.Hide();
			StartNewRun();
		}

		private bool CanExitNow()
		{
			return _zoneCalculator.IsSpecialZone(_currentZone) &&
			       !_isWheelSpinning;
		}

		private void RefreshExitAvailability()
		{
			ExitAvailabilityChanged?.Invoke(CanExitNow());
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

		// This is like a concept. I wanted to add ideas from myself for this demo.
		// I want to encourage the user to keep going by showing what's ahead.
		// It can be tweaked with your idea as well.
		public void OnHintClicked()
		{
			int nextZoneIndex = _currentZone + 1;

			if (nextZoneIndex > _gameSettings.EndGameZoneIndex)
			{
				return;
			}

			if (!_hasNextZoneWheel)
			{
				_nextZoneWheel = BuildZoneWheel(nextZoneIndex);
				_hasNextZoneWheel = true;
			}

			RewardDefinition bestReward = GetBestReward(_nextZoneWheel.Rewards);

			_exitPanelController.Hide();
			_hintPanelController.Show();
			_hintPanelController.DisplayHint(bestReward, GetRewardAmount(bestReward, nextZoneIndex));
		}

		public void OnHintClosed()
		{
			_hintPanelController.Hide();
			_exitPanelController.Show();
		}

		private RewardDefinition GetBestReward(List<RewardDefinition> rewards)
		{
			RewardDefinition bestReward = rewards.First();

			foreach (RewardDefinition reward in rewards)
			{
				if (IsBomb(reward))
				{
					continue;
				}

				if (reward.Rarity > bestReward.Rarity)
				{
					bestReward = reward;
				}
			}

			return bestReward;
		}

		public void DisplayEarnedRewards()
		{
			_rewardSummaryPanelController.Show();
			_rewardSummaryPanelController.DisplayEarnedRewards(_rewardContainer);
		}
	}
}
