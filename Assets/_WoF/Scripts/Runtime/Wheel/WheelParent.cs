using DG.Tweening;
using UnityEngine;
using WoF.Reward;

namespace WoF.Wheel
{
	public class WheelParent : MonoBehaviour, IWheelView
	{
		[SerializeField]
		private WheelSpin _wheelSpin;
		[SerializeField]
		private WheelIndicator _wheelIndicator;

		private void OnValidate()
		{
			_wheelSpin = GetComponentInChildren<WheelSpin>();
			_wheelIndicator = GetComponentInChildren<WheelIndicator>();
		}

		public void ApplyWheelType(WheelType wheelType)
		{
			if (wheelType == null)
			{
				return;
			}

			_wheelSpin.ApplyStyle(wheelType);
			_wheelIndicator.ApplyStyle(wheelType);
		}

		public void ApplySlot(int slotIndex, RewardDefinition reward, int amount)
		{
			_wheelSpin.ApplySlotView(slotIndex, reward, amount);
		}

		public void SetSlotActive(int slotIndex, bool active)
		{
			_wheelSpin.SetActiveWheelSlot(slotIndex, active);
		}

		public void ResetSlots()
		{
			_wheelSpin.ResetSlots();
		}

		public Tween PlaySpin(SpinResult spin, float duration, float settleDuration)
		{
			return _wheelSpin.PlaySpin(spin, duration, settleDuration);
		}
	}
}
