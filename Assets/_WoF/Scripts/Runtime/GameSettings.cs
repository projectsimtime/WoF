using UnityEngine;
using UnityEngine.Serialization;

namespace WoF
{
	[CreateAssetMenu(fileName = "gameSettings_", menuName = "WoF/Game Settings/ New Game Settings", order = 0)]
	public class GameSettings : ScriptableObject
	{
		[Header("Slot")]
		[SerializeField]
		private int _slotCount = 8;

		[Header("Zone")]
		[SerializeField]
		private int _safeZoneFrequency = 5;
		[SerializeField]
		private int _superZoneFrequency = 30;
		[SerializeField]
		private int _endGameZoneIndex = 60;

		[Header("Spin")]
		[SerializeField]
		private float _spinTargetAngle = 1980.0f;
		[SerializeField]
		private float _spinMinDuration = 3.1f;
		[SerializeField]
		private float _spinMaxDuration = 3.5f;

		[Header("Spin Bias")]
		[SerializeField]
		private float _spinSlotOffsetAngle = 20.0f;
		[Range(0.5f, 1.0f)]
		[SerializeField]
		private float _spinEdgeBias = 0.8f;
		[Range(0.0f, 1.0f)]
		[SerializeField]
		private float _spinNearMissChance = 0.5f;

		[Header("Rarity")]
		[Range(0, 100)]
		[SerializeField]
		private int _legendaryChance = 1;
		[Range(0, 100)]
		[SerializeField]
		private int _epicChance = 9;
		[Range(0, 100)]
		[SerializeField]
		private int _rareChance = 20;

		[Header("Currency")]
		[SerializeField]
		private int _startCurrencyAmount = 500;

		[Header("Revive")]
		[SerializeField]
		private int _reviveCostScale = 50;
		[SerializeField]
		private int _initReviveCost = 75;

		public int SlotCount => _slotCount;
		public float SlotAngle => 360.0f / _slotCount;

		public int SafeZoneFrequency => _safeZoneFrequency;
		public int SuperZoneFrequency => _superZoneFrequency;
		public int EndGameZoneIndex => _endGameZoneIndex;
		public int SuperZoneCount => _endGameZoneIndex / _superZoneFrequency;

		public float SpinTargetAngle => _spinTargetAngle;
		public float SpinMinDuration => _spinMinDuration;
		public float SpinMaxDuration => _spinMaxDuration;

		public float SpinSlotOffsetAngle => _spinSlotOffsetAngle;
		public float SpinEdgeBias => _spinEdgeBias;
		public float SpinNearMissChance => _spinNearMissChance;

		public int LegendaryChance => _legendaryChance;
		public int EpicChance => _epicChance;
		public int RareChance => _rareChance;

		public int StartCurrencyAmount => _startCurrencyAmount;

		public int ReviveCostScale => _reviveCostScale;
		public int InitReviveCost => _initReviveCost;
	}
}
