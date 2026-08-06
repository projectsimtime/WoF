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
		[SerializeField]
		private int _superZoneFrequency;
		[SerializeField]
		private int _endGameZoneIndex;

		public int SlotCount => _slotCount;
		public int SafeZoneFrequency => _safeZoneFrequency;
		public int SuperZoneFrequency => _superZoneFrequency;
		public int EndGameZoneIndex => _endGameZoneIndex;

		public int SuperZoneCount => _endGameZoneIndex / _superZoneFrequency;
	}
}