using System;
using DG.Tweening;
using UnityEngine;
using WoF.Currency;
using WoF.ExitPanel;
using WoF.LosePanel;
using WoF.Reward;
using WoF.Wheel;
using WoF.Zone;
using WoF.ZoneView;

namespace WoF
{
	public class GameSession : MonoBehaviour
	{
		[SerializeField]
		private ZoneRules _zoneRule;

		[SerializeField]
		[Tooltip("First ones have higher priority. Super zone is before safe zone so at 30 super zone will be selected.")]
		private ZoneTypeData[] _zoneTypes;

		[SerializeField]
		private WheelFlow _wheelFlow;

		[SerializeField]
		private GameSettings _gameSettings;

		[SerializeField]
		private RewardAmountMap _rewardAmountMap;

		[SerializeField]
		private ReviveFlow _reviveFlow;

		[SerializeField]
		private ExitFlow _exitFlow;

		[SerializeField]
		private RunRewardFlow _runRewardFlow;

		[SerializeField]
		private ZoneIndicatorPanelController _zoneIndicatorPanelController;

		[SerializeField]
		private CurrentZonePanelController _currentZonePanelController;

		[SerializeField]
		private CurrencyPanelController _currencyPanelController;

		private ZoneTypeCalculator _zoneTypeCalculator;
		private CurrencyContainer _currencyContainer;

		private int _currentZone;
		private bool _isWheelSpinning;

		private void Awake()
		{
			_zoneTypeCalculator = new ZoneTypeCalculator(_zoneTypes, _gameSettings.EndGameZoneIndex);
			_currencyContainer = new CurrencyContainer(_gameSettings.StartCurrencyAmount);

			_wheelFlow.Initialize(_zoneRule, _zoneTypeCalculator, _gameSettings, _rewardAmountMap);
			_wheelFlow.SpinCompleted += OnSpinComplete;
			_wheelFlow.SpinStateChanged += OnWheelSpinStateChanged;
			_reviveFlow.Initialize(_currencyContainer, _gameSettings.InitReviveCost, _gameSettings.ReviveCostScale);
			_reviveFlow.Revived += OnRevived;
			_reviveFlow.GaveUp += StartNewRun;
			_reviveFlow.CurrencyChanged += OnCurrencyChanged;
			_exitFlow.Initialize();
			_runRewardFlow.Initialize();
			_exitFlow.CollectRequested += _runRewardFlow.ShowEarnedRewards;
			_exitFlow.HintRequested += OnHintRequested;
			_runRewardFlow.NewRunRequested += StartNewRun;
		}

		private void OnDestroy()
		{
			_wheelFlow.SpinCompleted -= OnSpinComplete;
			_wheelFlow.SpinStateChanged -= OnWheelSpinStateChanged;
			_reviveFlow.Revived -= OnRevived;
			_reviveFlow.GaveUp -= StartNewRun;
			_reviveFlow.CurrencyChanged -= OnCurrencyChanged;
			_exitFlow.CollectRequested -= _runRewardFlow.ShowEarnedRewards;
			_exitFlow.HintRequested -= OnHintRequested;
			_runRewardFlow.NewRunRequested -= StartNewRun;
		}

		private void Start()
		{
			DOTween.Init();

			StartNewRun();
		}

		private void StartNewRun()
		{
			ClearRunState();
			EnterZone();
		}

		private void ClearRunState()
		{
			_currentZone = 1;
			RefreshExitAvailability();

			_wheelFlow.ResetRun();
			_runRewardFlow.ResetRun();
			_currencyContainer.Reset(_gameSettings.StartCurrencyAmount);
			_reviveFlow.ResetRun();

			_currencyPanelController.SetCurrencyAmount(_currencyContainer.GetRemainingCurrencyAmount());
			_exitFlow.ResetRun();
		}

		private void EnterZone()
		{
			RefreshExitAvailability();
			_wheelFlow.EnterZone(_currentZone);
			RefreshZoneIndicators();
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

		private bool IsGameFinished()
		{
			return _currentZone > _gameSettings.EndGameZoneIndex;
		}

		private void OnGameFinished()
		{
			_runRewardFlow.OnGameFinished();
		}

		private void RefreshZoneIndicators()
		{
			ZoneTypeData safeZoneType = _zoneTypeCalculator.GetZoneTypeDataFromIndex(_gameSettings.SafeZoneFrequency);
			ZoneTypeData superZoneType = _zoneTypeCalculator.GetZoneTypeDataFromIndex(_gameSettings.SuperZoneFrequency);

			int nextSafeZoneIndex = _zoneTypeCalculator.GetZonesNextIndex(safeZoneType, _currentZone);
			int nextSuperZoneIndex = _zoneTypeCalculator.GetZonesNextIndex(superZoneType, _currentZone);

			nextSafeZoneIndex = nextSafeZoneIndex != Int32.MaxValue ? nextSafeZoneIndex : _currentZone;
			nextSuperZoneIndex = nextSuperZoneIndex != Int32.MaxValue ? nextSuperZoneIndex : _currentZone;

			_currentZonePanelController.SetZoneIndex(_currentZone, _zoneTypeCalculator.GetZoneTypeDataFromIndex(_currentZone).ViewData.ThemeColor);

			_zoneIndicatorPanelController.OnNewZone(
				nextSafeZoneIndex,
				nextSuperZoneIndex,
				_wheelFlow.GetReservedReward(nextSuperZoneIndex).Sprite);
		}

		private void OnWheelSpinStateChanged(bool isSpinning)
		{
			_isWheelSpinning = isSpinning;

			RefreshExitAvailability();
		}

		private void OnSpinComplete(RewardDefinition earnedReward)
		{
			if (IsBomb(earnedReward))
			{
				OnBombExploded();
			}
			else
			{
				int earnedAmount = GetRewardAmount(earnedReward, _currentZone);

				_runRewardFlow.AddReward(earnedReward, earnedAmount);

				GoToNextZone();
			}
		}

		private bool IsBomb(RewardDefinition reward)
		{
			return reward && reward.Kind == EItemKind.Bomb;
		}

		private int GetRewardAmount(RewardDefinition reward, int zoneIndex)
		{
			return _rewardAmountMap.GetAmountByKind(reward, zoneIndex);
		}

		private void OnBombExploded()
		{
			_reviveFlow.OnBombExploded();
		}

		private void OnRevived()
		{
			_wheelFlow.OnRevived();
		}

		private void OnCurrencyChanged(int currencyAmount)
		{
			_currencyPanelController.SetCurrencyAmount(currencyAmount);
		}

		private bool CanExitNow()
		{
			return _zoneTypeCalculator.GetZoneTypeDataFromIndex(_currentZone).CanExit &&
			       !_isWheelSpinning;
		}

		private void RefreshExitAvailability()
		{
			_exitFlow.SetExitAvailability(CanExitNow());
		}

		// This is like a concept. I wanted to add ideas from myself for this demo.
		// I want to encourage the user to keep going by showing what's ahead.
		// It can be tweaked with your idea as well.
		private void OnHintRequested()
		{
			int nextZoneIndex = _currentZone + 1;

			if (nextZoneIndex > _gameSettings.EndGameZoneIndex)
			{
				return;
			}

			RewardDefinition bestReward = _wheelFlow.GetBestPreviewReward(nextZoneIndex);

			_exitFlow.OnHintClicked(bestReward, GetRewardAmount(bestReward, nextZoneIndex));
		}
	}
}
