using System;
using DG.Tweening;
using UnityEngine;
using WoF.Currency;
using WoF.EndGame;
using WoF.HintPanel;
using WoF.LosePanel;
using WoF.Reward;
using WoF.Zone;

namespace WoF.Session
{
	/// <summary>
	/// The sequencer. It answers one question, "what happens next", by handing off to the
	/// service that owns each concern. Every rule it needs already lives somewhere else.
	/// </summary>
	public class GameFlow : IRunCommands, ISpinCommands, IDebugRunCommands
	{
		private readonly RunState _runState;
		private readonly ZoneRunner _zoneRunner;
		private readonly SpinService _spinService;
		private readonly RewardService _rewardService;
		private readonly ReviveService _reviveService;
		private readonly ExitService _exitService;
		private readonly EconomyService _economyService;
		private readonly ZoneWheelFactory _zoneWheelFactory;
		private readonly IEndGameView _endGameView;
		private readonly IHintView _hintView;
		private readonly GameSettings _gameSettings;

		private Tween _pendingStepTween;

		public GameFlow(
			RunState runState,
			ZoneRunner zoneRunner,
			SpinService spinService,
			RewardService rewardService,
			ReviveService reviveService,
			ExitService exitService,
			EconomyService economyService,
			ZoneWheelFactory zoneWheelFactory,
			IEndGameView endGameView,
			IHintView hintView,
			GameSettings gameSettings)
		{
			_runState = runState;
			_zoneRunner = zoneRunner;
			_spinService = spinService;
			_rewardService = rewardService;
			_reviveService = reviveService;
			_exitService = exitService;
			_economyService = economyService;
			_zoneWheelFactory = zoneWheelFactory;
			_endGameView = endGameView;
			_hintView = hintView;
			_gameSettings = gameSettings;

			_spinService.SpinResolved += OnSpinResolved;
			_reviveService.GaveUp += OnGaveUp;
			_exitService.CollectRequested += ShowEarnedRewards;
		}

		public bool IsSpinning => _runState.IsSpinning;

		public event Action<bool> SpinningChanged
		{
			add => _runState.SpinningChanged += value;
			remove => _runState.SpinningChanged -= value;
		}

		public void StartNewRun()
		{
			CancelPendingStep();

			_runState.Reset();

			_rewardService.Clear();
			_rewardService.HideSummary();
			_economyService.ResetForNewRun();
			_reviveService.ResetForNewRun();

			_endGameView.Hide();
			_hintView.Hide();

			_exitService.Refresh();

			_zoneRunner.PrepareNewRun();
			_zoneRunner.EnterZone();
		}

		public void ShowEarnedRewards()
		{
			_rewardService.ShowSummary();
		}

		public void RequestSpin()
		{
			_spinService.Spin();
		}

		public void JumpToZone(int zoneIndex)
		{
			CancelPendingStep();

			_zoneWheelFactory.InvalidateCache();

			_runState.IsResumingAfterBomb = false;
			_runState.SetZone(Mathf.Clamp(zoneIndex, RunState.FirstZoneIndex, _gameSettings.EndGameZoneIndex));

			_zoneRunner.EnterZone();
		}

		private void OnSpinResolved(int slotIndex)
		{
			RewardDefinition reward = _zoneRunner.GetReward(slotIndex);

			if (reward == null)
			{
				AdvanceZone();

				return;
			}

			if (reward.IsBomb)
			{
				// A beat between the wheel stopping and the bomb screen, so the player reads
				// what they landed on before the panel covers it.
				ScheduleStep(_gameSettings.BombRevealDelay, _reviveService.ShowLoseScreen);

				return;
			}

			_rewardService.Collect(reward, _runState.CurrentZone);

			ScheduleStep(_gameSettings.RewardRevealDelay, AdvanceZone);
		}

		private void AdvanceZone()
		{
			_runState.AdvanceZone();

			if (_zoneRunner.IsRunFinished)
			{
				_endGameView.Show();

				return;
			}

			_zoneRunner.EnterZone();
		}

		/// <summary>
		/// Giving up ends the run on the summary screen instead of dropping the player straight
		/// back onto a fresh wheel.
		/// </summary>
		private void OnGaveUp()
		{
			ShowEarnedRewards();
		}

		private void ScheduleStep(float delay, TweenCallback step)
		{
			CancelPendingStep();

			if (delay <= 0.0f)
			{
				step();

				return;
			}

			_pendingStepTween = DOVirtual.DelayedCall(delay, () =>
			{
				_pendingStepTween = null;

				step();
			}, false);
		}

		private void CancelPendingStep()
		{
			if (_pendingStepTween == null)
			{
				return;
			}

			Tween pendingStepTween = _pendingStepTween;
			_pendingStepTween = null;

			pendingStepTween.Kill();
		}
	}
}
