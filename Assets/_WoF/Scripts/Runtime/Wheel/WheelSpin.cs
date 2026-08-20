using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WoF.Interface;
using WoF.Reward;

namespace WoF.Wheel
{
	public class WheelSpin : MonoBehaviour, IStyle<WheelType>
	{
		private const float MinimumFullTurns = 1.0f;

		[SerializeField]
		private WheelSlotView[] _wheelSlotViews;

		[SerializeField]
		private Image _image;

		private Sequence _spinSequence;

		private void OnValidate()
		{
			_wheelSlotViews = GetComponentsInChildren<WheelSlotView>();

			_image = GetComponent<Image>();
		}

		/// <summary>
		/// Spins on from wherever the wheel currently sits instead of snapping back to zero
		/// between zones, then eases the last few degrees into the centre of the winning slot.
		/// </summary>
		public Tween PlaySpin(SpinResult spin, float duration, float settleDuration)
		{
			KillSpin();

			float currentAngle = transform.localEulerAngles.z;
			float fullTurns = Mathf.Max(MinimumFullTurns, Mathf.Floor(Mathf.Abs(spin.TargetAngle) / 360.0f));

			float spinRotation = Mathf.Repeat(spin.TargetAngle - currentAngle, 360.0f) + (fullTurns * 360.0f);
			float settleRotation = Mathf.DeltaAngle(spin.TargetAngle, spin.SettleAngle);

			Sequence sequence = DOTween.Sequence();

			sequence.Append(transform
				.DOLocalRotate(new Vector3(0.0f, 0.0f, spinRotation), duration, RotateMode.LocalAxisAdd)
				.SetEase(Ease.OutCubic));

			if (settleDuration > 0.0f && !Mathf.Approximately(settleRotation, 0.0f))
			{
				sequence.Append(transform
					.DOLocalRotate(new Vector3(0.0f, 0.0f, settleRotation), settleDuration, RotateMode.LocalAxisAdd)
					.SetEase(Ease.InOutSine));
			}

			sequence.OnKill(() => _spinSequence = null);

			_spinSequence = sequence;

			return sequence;
		}

		private void KillSpin()
		{
			if (_spinSequence == null)
			{
				return;
			}

			Sequence spinSequence = _spinSequence;
			_spinSequence = null;

			spinSequence.Kill();
		}

		public void ApplyStyle(WheelType style)
		{
			_image.sprite = style.WheelSprite;
		}

		public void ApplySlotView(int index, RewardDefinition reward, int amount)
		{
			if (!IsSlotIndexValid(index) || reward == null)
			{
				return;
			}

			_wheelSlotViews[index].SetSprite(reward.Sprite);
			_wheelSlotViews[index].SetAmount(amount);
		}

		public void SetActiveWheelSlot(int index, bool active)
		{
			if (!IsSlotIndexValid(index))
			{
				return;
			}

			_wheelSlotViews[index].gameObject.SetActive(active);
		}

		public void ResetSlots()
		{
			for (int i = 0; i < _wheelSlotViews.Length; ++i)
			{
				_wheelSlotViews[i].gameObject.SetActive(true);
			}
		}

		private bool IsSlotIndexValid(int index)
		{
			return index >= 0 && index < _wheelSlotViews.Length;
		}
	}
}
