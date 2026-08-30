using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using WoF.Currency;
using WoF.ExitPanel;
using WoF.LosePanel;
using WoF.Reward;
using WoF.UI;
using WoF.Wheel;
using WoF.Zone;

namespace WoF
{
	public class GameSession : MonoBehaviour
	{
		[SerializeField]
		private ZoneRules _zoneRules;

		[SerializeField]
		private ZonePattern _zonePattern;

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
		private StartScreen _startScreen;

		private ZoneSchedule _zoneSchedule;
		private CurrencyContainer _currencyContainer;

		private int _currentZoneIndex;
		private bool _isWheelSpinning;
		private bool _isRewardRevealing;

		public event Action<int, ZoneDefinition> ZoneEntered;
		public event Action<ZoneDefinition, int, RewardDefinition> UpcomingSpecialZoneChanged;
		public event Action<int> CurrencyChanged;

		public ZoneSchedule ZoneSchedule => _zoneSchedule;

		private void Awake()
		{
			_zoneSchedule = new ZoneSchedule(_zonePattern, _gameSettings.EndGameZoneIndex);
			_currencyContainer = new CurrencyContainer(_gameSettings.StartCurrencyAmount);

			_wheelFlow.Initialize(_zoneRules, _zoneSchedule, _gameSettings, _rewardAmountMap);
			_wheelFlow.SpinCompleted += OnSpinComplete;
			_wheelFlow.SpinStateChanged += OnWheelSpinStateChanged;
			_reviveFlow.Initialize(_currencyContainer, _gameSettings.InitReviveCost, _gameSettings.ReviveCostScale, _gameSettings.LosePanelScaleDuration);
			_reviveFlow.Revived += OnRevived;
			_reviveFlow.GaveUp += ShowStartScreen;
			_reviveFlow.CurrencyChanged += OnCurrencyChanged;
			_exitFlow.Initialize();
			_runRewardFlow.Initialize();
			_startScreen.Initialize();
			_exitFlow.CollectRequested += _runRewardFlow.ShowEarnedRewards;
			_exitFlow.HintRequested += OnHintRequested;
			_runRewardFlow.NewRunRequested += ShowStartScreen;
			_startScreen.PlayRequested += StartNewRun;
		}

		private void OnDestroy()
		{
			_wheelFlow.SpinCompleted -= OnSpinComplete;
			_wheelFlow.SpinStateChanged -= OnWheelSpinStateChanged;
			_reviveFlow.Revived -= OnRevived;
			_reviveFlow.GaveUp -= ShowStartScreen;
			_reviveFlow.CurrencyChanged -= OnCurrencyChanged;
			_exitFlow.CollectRequested -= _runRewardFlow.ShowEarnedRewards;
			_exitFlow.HintRequested -= OnHintRequested;
			_runRewardFlow.NewRunRequested -= ShowStartScreen;
			_startScreen.PlayRequested -= StartNewRun;
		}

		private void Start()
		{
			DOTween.Init();

			ShowStartScreen();
		}

		private void StartNewRun()
		{
			_startScreen.Hide();
			ClearRunState();
			EnterZone();
		}

		private void ShowStartScreen()
		{
			_startScreen.Show();
		}

		private void ClearRunState()
		{
			_currentZoneIndex = 1;
			_isRewardRevealing = false;
			RefreshExitAvailability();

			_wheelFlow.ResetRun();
			_runRewardFlow.ResetRun();
			_currencyContainer.Reset(_gameSettings.StartCurrencyAmount);
			_reviveFlow.ResetRun();

			CurrencyChanged?.Invoke(_currencyContainer.GetRemainingCurrencyAmount());
			_exitFlow.ResetRun();
		}

		private void EnterZone()
		{
			RefreshExitAvailability();
			_wheelFlow.EnterZone(_currentZoneIndex);

			ZoneDefinition currentZoneDefinition = _zoneSchedule.GetZoneDefinition(_currentZoneIndex);
			ZoneEntered?.Invoke(_currentZoneIndex, currentZoneDefinition);
			NotifyUpcomingSpecialZones();
		}

		private void GoToNextZone()
		{
			++_currentZoneIndex;

			if (IsGameFinished())
			{
				OnGameFinished();
				return;
			}

			EnterZone();
		}

		private bool IsGameFinished()
		{
			return _currentZoneIndex > _gameSettings.EndGameZoneIndex;
		}

		private void OnGameFinished()
		{
			_runRewardFlow.OnGameFinished();
		}

		private void NotifyUpcomingSpecialZones()
		{
			foreach (ZoneDefinition zoneDefinition in _zoneSchedule.SpecialZoneDefinitions)
			{
				int nextZoneIndex = _zoneSchedule.GetNextZoneIndex(zoneDefinition, _currentZoneIndex);

				UpcomingSpecialZoneChanged?.Invoke(
					zoneDefinition,
					nextZoneIndex,
					nextZoneIndex == Int32.MaxValue ? null : _wheelFlow.GetReservedReward(nextZoneIndex));
			}
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
				StartCoroutine(OnBombExploded());
			}
			else
			{
				int earnedAmount = GetRewardAmount(earnedReward, _currentZoneIndex);

				StartCoroutine(RevealReward(earnedReward, earnedAmount));
			}
		}

		private IEnumerator RevealReward(RewardDefinition reward, int amount)
		{
			_isRewardRevealing = true;
			_wheelFlow.SetSpinInteractable(false);
			RefreshExitAvailability();

			yield return _runRewardFlow.RevealReward(reward, amount);

			_isRewardRevealing = false;
			_wheelFlow.SetSpinInteractable(true);
			GoToNextZone();
		}

		private bool IsBomb(RewardDefinition reward)
		{
			return reward && reward.Kind && reward.Kind.IsExplosive;
		}

		private int GetRewardAmount(RewardDefinition reward, int zoneIndex)
		{
			return _rewardAmountMap.GetAmountByKind(reward, zoneIndex);
		}

		private IEnumerator OnBombExploded()
		{
			_wheelFlow.SetSpinInteractable(false);

			yield return _wheelFlow.PlayBombReaction().WaitForCompletion();

			_reviveFlow.OnBombExploded();
		}

		private void OnRevived()
		{
			_wheelFlow.OnRevived();
			_wheelFlow.SetSpinInteractable(true);
		}

		private void OnCurrencyChanged(int currencyAmount)
		{
			CurrencyChanged?.Invoke(currencyAmount);
		}

		private bool CanExitNow()
		{
			ZoneDefinition currentZoneDefinition = _zoneSchedule.GetZoneDefinition(_currentZoneIndex);

			return currentZoneDefinition && currentZoneDefinition.CanExit &&
			       !_isWheelSpinning &&
			       !_isRewardRevealing;
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
			int nextZoneIndex = _currentZoneIndex + 1;

			if (nextZoneIndex > _gameSettings.EndGameZoneIndex)
			{
				return;
			}

			RewardDefinition bestReward = _wheelFlow.GetBestPreviewReward(nextZoneIndex);

			_exitFlow.OnHintClicked(bestReward, GetRewardAmount(bestReward, nextZoneIndex));
		}
	}
}
