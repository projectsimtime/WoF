using UnityEngine;

namespace WoF.Wheel
{
	public class SpinAngleCalculator
	{
		private int _slotCount;

		private float _spinTargetAngle;
		private float _spinSlotOffsetAngle;
		private float _spinEdgeBias;
		private float _spinNearMissChance;
		
		private float SlotAngle => 360.0f / _slotCount;

		public SpinAngleCalculator(int slotCount, float spinTargetAngle, float spinSlotOffsetAngle, float spinEdgeBias, float spinNearMissChance)
		{
			_slotCount = slotCount;

			_spinTargetAngle = spinTargetAngle;
			_spinSlotOffsetAngle = spinSlotOffsetAngle;
			_spinEdgeBias = spinEdgeBias;
			_spinNearMissChance = spinNearMissChance;
		}

		public float CalculateTargetAngle(int reservedSlotIndex, bool shouldSkipReservedIndex, out int randomSlotIndex)
		{
			randomSlotIndex = GetRandomSlotIndex(reservedSlotIndex, shouldSkipReservedIndex);
			float angle;

			float edgeBias = _spinEdgeBias;
			float innerBias = 1.0f - edgeBias;

			if (!shouldSkipReservedIndex && IsRandomIndexCloseToReservedSlot(randomSlotIndex, reservedSlotIndex))
			{
				bool shouldLookLikeShowNearHit = (randomSlotIndex + 1) % _slotCount == reservedSlotIndex;

				if (shouldLookLikeShowNearHit)
				{
					angle = GetAngleFromIndex(randomSlotIndex, edgeBias);
				}
				else
				{
					angle = GetAngleFromIndex(randomSlotIndex, innerBias);
				}

			}
			else if(reservedSlotIndex == randomSlotIndex)
			{
				bool shouldLookLikeShowNearMiss = GetProbability(_spinNearMissChance);

				angle = GetAngleFromIndex(randomSlotIndex, shouldLookLikeShowNearMiss ? edgeBias : innerBias);
			}
			else
			{
				float alpha = Random.Range(innerBias, edgeBias);

				angle = GetAngleFromIndex(randomSlotIndex, alpha);
			}

			return angle;
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

		private float GetAngleFromIndex(int index, float alpha = 0.5f)
		{
			float slotPosition = index * SlotAngle;
			float slotOffset = _spinSlotOffsetAngle;

			int turnCount = Mathf.RoundToInt((_spinTargetAngle + slotPosition) / 360.0f);

			// Its -slotPosition because slots are positioned CCW.
			return (turnCount * 360.0f) - slotPosition + Mathf.Lerp(-slotOffset, slotOffset, alpha);
		}

		private bool IsRandomIndexCloseToReservedSlot(int index, int reservedSlotIndex)
		{
			return (index + 1 + _slotCount) % _slotCount == reservedSlotIndex ||
			       (index - 1 + _slotCount) % _slotCount == reservedSlotIndex;
		}
	}
}
