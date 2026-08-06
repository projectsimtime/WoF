using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using WoF.Reward;

namespace WoF
{
	public class WheelParent : MonoBehaviour
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
			_wheelSpin.ApplyStyle(wheelType);
			_wheelIndicator.ApplyStyle(wheelType);
		}

		public void ApplyWheelSlot(int index, RewardDefinition reward, int amount)
		{
			_wheelSpin.ApplySlotView(index, reward, amount);
		}

		public void SetActiveWheelSlot(int index, bool active)
		{
			_wheelSpin.SetActiveWheelSlot(index, active);
		}

		public void RestoreWheelRotation()
		{
			_wheelSpin.SetRotation(Vector3.zero);
		}

		public TweenerCore<Quaternion, Vector3, QuaternionOptions> PlaySpin(float targetAngle, float duration)
		{
			return _wheelSpin.PlaySpin(targetAngle, duration);
		}
	}
}