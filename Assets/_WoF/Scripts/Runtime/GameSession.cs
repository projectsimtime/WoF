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
		[SerializeField]
		private ZoneRules _zoneRule;

		[SerializeField]
		private ZoneTypeData[] _zoneTypes;

		[SerializeField]
		private WheelParent _wheelParent;

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

		private ZoneCalculator _zoneCalculator;
		private ZoneWheelBuilder _zoneWheelBuilder;
		private RunEconomy _runEconomy;
		private SpinAngleCalculator _spinAngleCalculator;

		private EarnedRewardContainer _rewardContainer;

		private List<RewardDefinition> _rewardDefinitions = new();
		private RewardDefinition _earnedReward;

		private ZoneWheelData _nextZoneWheel;
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
			_zoneWheelBuilder = new ZoneWheelBuilder(_zoneRule, _zoneTypes, _gameSettings);
			_runEconomy = new RunEconomy(_gameSettings.StartCurrencyAmount, _gameSettings.InitReviveCost, _gameSettings.ReviveCostScale);
			_spinAngleCalculator = new SpinAngleCalculator(_gameSettings.SlotCount, _gameSettings.SpinTargetAngle, _gameSettings.SpinSlotOffsetAngle, _gameSettings.SpinEdgeBias, _gameSettings.SpinNearMissChance);
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
			_runEconomy.Reset();

			_losePanelController.ShowAdButton();
			_losePanelController.UpdateReviveCost(_runEconomy.GetReviveCost());
			_currencyPanelController.SetCurrencyAmount(_runEconomy.GetRemainingCurrencyAmount());
			_zoneWheelBuilder.Reset();

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

			ZoneWheelData zoneWheel = GetZoneWheel(_currentZone);

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

			_currentZonePanelController.SetZoneIndex(_currentZone, _zoneWheelBuilder.GetZoneTypeData(_currentZone).ViewData.ThemeColor);

			_zoneIndicatorPanelController.OnNewZone(
				_zoneCalculator.GetNextSafeZoneIndex(_currentZone),
				nextSuperZoneIndex,
				_zoneWheelBuilder.GetSuperZoneReward(nextSuperZoneIndex).Sprite);
		}

		private ZoneWheelData GetZoneWheel(int zoneIndex)
		{
			if (_hasNextZoneWheel)
			{
				_hasNextZoneWheel = false;

				return _nextZoneWheel;
			}

			return _zoneWheelBuilder.BuildZoneWheel(zoneIndex);
		}

		private void ApplyZoneWheel(ZoneWheelData zoneWheel, int zoneIndex)
		{
			_wheelParent.ApplyWheelType(zoneWheel.WheelType);

			for (int i = 0; i < zoneWheel.Rewards.Count; ++i)
			{
				_wheelParent.ApplyWheelSlot(i, zoneWheel.Rewards[i], GetRewardAmount(zoneWheel.Rewards[i], zoneIndex));
			}
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
			return reward.Kind == EItemKind.Bomb;
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

			_runEconomy.PayReviveCost();

			int nextReviveCost = _runEconomy.GetReviveCost();
			_losePanelController.UpdateReviveCost(nextReviveCost);

			if (!_runEconomy.HasEnoughCurrencyForRevive())
			{
				_losePanelController.OnNotEnoughCurrencyToRevive();
			}

			_currencyPanelController.SetCurrencyAmount(_runEconomy.GetRemainingCurrencyAmount());
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
				_nextZoneWheel = _zoneWheelBuilder.BuildZoneWheel(nextZoneIndex);
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
