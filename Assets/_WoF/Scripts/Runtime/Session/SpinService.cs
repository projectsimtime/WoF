using System;
using DG.Tweening;
using WoF.Wheel;
using Random = UnityEngine.Random;

namespace WoF.Session
{
	/// <summary>
	/// Turns a spin request into a wheel animation and reports which slot it stopped on.
	/// It decides where the wheel lands, not what landing there means.
	/// </summary>
	public class SpinService
	{
		private readonly SpinAngleCalculator _spinAngleCalculator;
		private readonly IWheelView _wheelView;
		private readonly RunState _runState;
		private readonly float _minDuration;
		private readonly float _maxDuration;
		private readonly float _settleDuration;

		public SpinService(
			SpinAngleCalculator spinAngleCalculator,
			IWheelView wheelView,
			RunState runState,
			float minDuration,
			float maxDuration,
			float settleDuration)
		{
			_spinAngleCalculator = spinAngleCalculator;
			_wheelView = wheelView;
			_runState = runState;
			_minDuration = minDuration;
			_maxDuration = maxDuration;
			_settleDuration = settleDuration;
		}

		public event Action<int> SpinResolved;

		public void Spin()
		{
			if (_runState.IsSpinning)
			{
				return;
			}

			_runState.SetSpinning(true);

			SpinResult spin = _spinAngleCalculator.CalculateSpin(_runState.ReservedSlotIndex, _runState.IsResumingAfterBomb);

			float duration = Random.Range(_minDuration, _maxDuration);

			Tween spinTween = _wheelView.PlaySpin(spin, duration, _settleDuration);

			if (spinTween == null)
			{
				Complete(spin.SlotIndex);

				return;
			}

			spinTween.OnComplete(() => Complete(spin.SlotIndex));
		}

		private void Complete(int slotIndex)
		{
			_runState.SetSpinning(false);

			SpinResolved?.Invoke(slotIndex);
		}
	}
}
