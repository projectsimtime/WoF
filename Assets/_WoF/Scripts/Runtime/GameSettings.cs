using UnityEngine;

namespace WoF
{
	[CreateAssetMenu(fileName = "gameSettings_", menuName = "WoF/Game Settings/ New Game Settings", order = 0)]
	public class GameSettings : ScriptableObject
	{
		[Header("Slot")]
		[SerializeField]
		private int _slotCount;
		[Header("Zone")]
		[SerializeField] 
		private int _safeZoneFrequency;
		[Header("Zone")]
		[SerializeField] 
		private int _superZoneFrequency;

		public int SlotCount => _slotCount;
		public int SafeZoneFrequency => _safeZoneFrequency;
		public int SuperZoneFrequency => _superZoneFrequency;
	}
}