using UnityEngine;

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

		[Header("Currency")]
		[SerializeField]
		private int _startCurrencyAmount = 500;

		[Header("Revive")]
		[SerializeField]
		private int _reviveCostScale = 50;
		[SerializeField]
		private int _initReviveCost = 75;

		[Header("Reveal")]
		[SerializeField]
		private float _revealBlockerFadeDuration = 0.2f;
		[SerializeField]
		private float _revealCardScaleDuration = 0.35f;
		[SerializeField]
		private float _revealRailFlightDuration = 0.45f;
		[SerializeField]
		private float _revealLandingDuration = 0.2f;
		[SerializeField]
		private float _revealCounterDuration = 0.6f;
		[Range(0.1f, 1.0f)]
		[SerializeField]
		private float _revealHeroHeightRatio = 0.6f;
		[SerializeField]
		private float _explosiveSlotFlashDuration = 0.15f;
		[SerializeField]
		private float _explosiveWheelShakeDuration = 0.25f;
		[SerializeField]
		private float _explosivePanelDelay = 0.15f;
		[SerializeField]
		private float _losePanelFadeDuration = 0.25f;

		public int SlotCount => _slotCount;
		public float SlotAngle => 360.0f / _slotCount;

		public int EndGameZoneIndex => _endGameZoneIndex;

		public float SpinTargetAngle => _spinTargetAngle;
		public float SpinMinDuration => _spinMinDuration;
		public float SpinMaxDuration => _spinMaxDuration;

		public float SpinSlotOffsetAngle => _spinSlotOffsetAngle;
		public float SpinEdgeBias => _spinEdgeBias;
		public float SpinNearMissChance => _spinNearMissChance;

		public int StartCurrencyAmount => _startCurrencyAmount;

		public int ReviveCostScale => _reviveCostScale;
		public int InitReviveCost => _initReviveCost;

		public float RevealBlockerFadeDuration => _revealBlockerFadeDuration;
		public float RevealCardScaleDuration => _revealCardScaleDuration;
		public float RevealRailFlightDuration => _revealRailFlightDuration;
		public float RevealLandingDuration => _revealLandingDuration;
		public float RevealCounterDuration => _revealCounterDuration;
		public float RevealHeroHeightRatio => _revealHeroHeightRatio;
		public float ExplosiveSlotFlashDuration => _explosiveSlotFlashDuration;
		public float ExplosiveWheelShakeDuration => _explosiveWheelShakeDuration;
		public float ExplosivePanelDelay => _explosivePanelDelay;
		public float LosePanelFadeDuration => _losePanelFadeDuration;
	}
}
