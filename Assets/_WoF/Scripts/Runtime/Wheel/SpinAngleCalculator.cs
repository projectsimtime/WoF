using UnityEngine;

namespace WoF.Wheel
{
	public class SpinAngleCalculator
	{
		private const float SlotCentreAlpha = 0.5f;

		private readonly int _slotCount;

		private readonly float _spinTargetAngle;
		private readonly float _spinSlotOffsetAngle;
		private readonly float _spinEdgeBias;
		private readonly float _spinNearMissChance;

		private float SlotAngle => 360.0f / _slotCount;

		public SpinAngleCalculator(int slotCount, float spinTargetAngle, float spinSlotOffsetAngle, float spinEdgeBias, float spinNearMissChance)
		{
			_slotCount = slotCount;

			_spinTargetAngle = spinTargetAngle;
			_spinSlotOffsetAngle = spinSlotOffsetAngle;
			_spinEdgeBias = spinEdgeBias;
			_spinNearMissChance = spinNearMissChance;
		}

		public SpinResult CalculateSpin(int reservedSlotIndex, bool shouldSkipReservedIndex)
		{
			int slotIndex = GetRandomSlotIndex(reservedSlotIndex, shouldSkipReservedIndex);

			float alpha = GetSlotAlpha(slotIndex, reservedSlotIndex, shouldSkipReservedIndex);

			return new SpinResult
			{
				SlotIndex = slotIndex,
				TargetAngle = GetAngleFromIndex(slotIndex, alpha),
				SettleAngle = GetAngleFromIndex(slotIndex, SlotCentreAlpha)
			};
		}

		/// <summary>
		/// How far inside its slot the wheel is allowed to stop. Landing next to the bomb leans
		/// towards the bomb, landing on the bomb sometimes leans away from it, everything else
		/// stays comfortably inside the slot.
		/// </summary>
		private float GetSlotAlpha(int slotIndex, int reservedSlotIndex, bool shouldSkipReservedIndex)
		{
			float edgeBias = _spinEdgeBias;
			float innerBias = 1.0f - edgeBias;

			if (!shouldSkipReservedIndex && IsNextToReservedSlot(slotIndex, reservedSlotIndex))
			{
				bool stopsJustBeforeReservedSlot = (slotIndex + 1) % _slotCount == reservedSlotIndex;

				return stopsJustBeforeReservedSlot ? edgeBias : innerBias;
			}

			if (slotIndex == reservedSlotIndex)
			{
				return GetProbability(_spinNearMissChance) ? edgeBias : innerBias;
			}

			return Random.Range(innerBias, edgeBias);
		}

		private int GetRandomSlotIndex(int reservedIndex, bool shouldSkipReservedIndex)
		{
			int randomSlotIndex = Random.Range(0, _slotCount);

			if (shouldSkipReservedIndex && randomSlotIndex == reservedIndex)
			{
				randomSlotIndex = (randomSlotIndex + 1) % _slotCount;
			}

			return randomSlotIndex;
		}

		private bool GetProbability(float alpha)
		{
			return Random.value < alpha;
		}

		private float GetAngleFromIndex(int index, float alpha)
		{
			float slotPosition = index * SlotAngle;
			float slotOffset = _spinSlotOffsetAngle;

			int turnCount = Mathf.RoundToInt((_spinTargetAngle + slotPosition) / 360.0f);

			// Its -slotPosition because slots are positioned CCW.
			return (turnCount * 360.0f) - slotPosition + Mathf.Lerp(-slotOffset, slotOffset, alpha);
		}

		private bool IsNextToReservedSlot(int index, int reservedSlotIndex)
		{
			return (index + 1 + _slotCount) % _slotCount == reservedSlotIndex ||
			       (index - 1 + _slotCount) % _slotCount == reservedSlotIndex;
		}
	}
}
