namespace WoF.Wheel
{
	/// <summary>
	/// Where a spin should stop. <see cref="TargetAngle"/> is the biased angle that sells the
	/// near miss, <see cref="SettleAngle"/> is the exact centre of the slot the wheel eases into
	/// afterwards, so the indicator always ends up reading the reward it landed on.
	/// </summary>
	public struct SpinResult
	{
		public int SlotIndex;
		public float TargetAngle;
		public float SettleAngle;
	}
}
