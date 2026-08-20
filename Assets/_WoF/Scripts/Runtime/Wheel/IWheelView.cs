using DG.Tweening;
using WoF.Core;
using WoF.Reward;

namespace WoF.Wheel
{
	/// <summary>
	/// Everything the game logic is allowed to ask of the wheel. The logic never touches
	/// a concrete MonoBehaviour, which is what makes the spin flow testable on its own.
	/// </summary>
	public interface IWheelView : ISceneService
	{
		void ApplyWheelType(WheelType wheelType);

		void ApplySlot(int slotIndex, RewardDefinition reward, int amount);

		void SetSlotActive(int slotIndex, bool active);

		void ResetSlots();

		Tween PlaySpin(SpinResult spin, float duration, float settleDuration);
	}
}
