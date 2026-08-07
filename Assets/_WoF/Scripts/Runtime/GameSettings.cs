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
		[Header("Zone")]
		[SerializeField]
		private int _endGameZoneIndex;

		[SerializeField] 
		private int _startCurrencyAmount;
		[SerializeField]
		private int _reviveCostScale;
		[SerializeField]
		private int _initReviveCost;

		public int SlotCount => _slotCount;
		public int SafeZoneFrequency => _safeZoneFrequency;
		public int SuperZoneFrequency => _superZoneFrequency;
		public int EndGameZoneIndex => _endGameZoneIndex;
		
		public int SuperZoneCount => _endGameZoneIndex / _superZoneFrequency;
		
		public int StartCurrencyAmount => _startCurrencyAmount;
		public int ReviveCostScale => _reviveCostScale;
		public int InitReviveCost => _initReviveCost;
		
	}
}