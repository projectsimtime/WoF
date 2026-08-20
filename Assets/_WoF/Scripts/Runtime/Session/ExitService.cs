using System;
using WoF.ExitPanel;
using WoF.Zone;

namespace WoF.Session
{
	/// <summary>
	/// Decides when leaving is allowed and drives the exit screen. It listens to the run
	/// instead of being poked, so no caller has to remember to refresh the exit button.
	/// </summary>
	public class ExitService : IExitCommands
	{
		private readonly ZoneCalculator _zoneCalculator;
		private readonly RunState _runState;
		private readonly IExitView _exitView;

		public ExitService(ZoneCalculator zoneCalculator, RunState runState, IExitView exitView)
		{
			_zoneCalculator = zoneCalculator;
			_runState = runState;
			_exitView = exitView;

			_runState.ZoneChanged += OnZoneChanged;
			_runState.SpinningChanged += OnSpinningChanged;
		}

		public bool IsExitAvailable { get; private set; }

		public event Action<bool> ExitAvailabilityChanged;
		public event Action CollectRequested;

		public void RequestExit()
		{
			if (!IsExitAvailable)
			{
				return;
			}

			_exitView.Show();
		}

		public void CancelExit()
		{
			_exitView.Hide();
		}

		public void CollectAndExit()
		{
			_exitView.Hide();

			CollectRequested?.Invoke();
		}

		public void Refresh()
		{
			bool isExitAvailable = _zoneCalculator.CanExitAt(_runState.CurrentZone) && !_runState.IsSpinning;

			if (isExitAvailable == IsExitAvailable)
			{
				return;
			}

			IsExitAvailable = isExitAvailable;

			ExitAvailabilityChanged?.Invoke(isExitAvailable);
		}

		private void OnZoneChanged(int zoneIndex)
		{
			Refresh();
		}

		private void OnSpinningChanged(bool isSpinning)
		{
			Refresh();
		}
	}
}
