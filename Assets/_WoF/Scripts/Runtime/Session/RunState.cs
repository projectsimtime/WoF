using System;
using WoF.Zone;

namespace WoF.Session
{
	/// <summary>
	/// The mutable facts about the run in progress. Services read and change it, views listen
	/// to it. Keeping it in one small object is what let the flow classes stay stateless.
	/// </summary>
	public class RunState
	{
		public const int FirstZoneIndex = ZoneCalculator.FirstZoneIndex;

		public int CurrentZone { get; private set; } = FirstZoneIndex;

		/// <summary>
		/// The slot holding the bomb or the special reward, or <see cref="ZoneWheel.NoReservedSlot"/>.
		/// </summary>
		public int ReservedSlotIndex { get; set; } = ZoneWheel.NoReservedSlot;

		/// <summary>
		/// True while the player is playing the extra spin a revive bought them.
		/// </summary>
		public bool IsResumingAfterBomb { get; set; }

		public bool IsSpinning { get; private set; }

		public event Action<int> ZoneChanged;
		public event Action<bool> SpinningChanged;

		public void SetZone(int zoneIndex)
		{
			CurrentZone = zoneIndex;

			ZoneChanged?.Invoke(zoneIndex);
		}

		public void AdvanceZone()
		{
			SetZone(CurrentZone + 1);
		}

		public void SetSpinning(bool isSpinning)
		{
			if (IsSpinning == isSpinning)
			{
				return;
			}

			IsSpinning = isSpinning;

			SpinningChanged?.Invoke(isSpinning);
		}

		public void Reset()
		{
			ReservedSlotIndex = ZoneWheel.NoReservedSlot;
			IsResumingAfterBomb = false;

			SetSpinning(false);
			SetZone(FirstZoneIndex);
		}
	}
}
