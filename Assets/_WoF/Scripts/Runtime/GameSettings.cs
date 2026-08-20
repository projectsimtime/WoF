using System.Collections.Generic;
using UnityEngine;
using WoF.Advertisement;
using WoF.Reward;
using WoF.Wheel;

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
		[Tooltip("Time the wheel takes to ease into the centre of the slot it stopped on.")]
		[SerializeField]
		private float _spinSettleDuration = 0.22f;

		[Header("Spin Bias")]
		[SerializeField]
		private float _spinSlotOffsetAngle = 14.0f;
		[Range(0.5f, 1.0f)]
		[SerializeField]
		private float _spinEdgeBias = 0.8f;
		[Range(0.0f, 1.0f)]
		[SerializeField]
		private float _spinNearMissChance = 0.5f;

		[Header("Pacing")]
		[Tooltip("Beat between winning a reward and the next zone being dealt.")]
		[SerializeField]
		private float _rewardRevealDelay = 0.45f;
		[Tooltip("Beat between landing on the bomb and the bomb screen.")]
		[SerializeField]
		private float _bombRevealDelay = 0.6f;

		[Header("Rarity")]
		[Tooltip("Evaluated from the top down, so the rarest rarity belongs first.")]
		[SerializeField]
		private RarityChance[] _rarityChances;
		[Tooltip("Drawn when no row of the table wins the roll.")]
		[SerializeField]
		private EItemRarity _baseRarity = EItemRarity.Casual;

		[Header("Currency")]
		[SerializeField]
		private int _startCurrencyAmount = 500;

		[Header("Revive")]
		[SerializeField]
		private int _reviveCostScale = 50;
		[SerializeField]
		private int _initReviveCost = 75;

		[Header("Advertisement")]
		[SerializeField]
		private AdPlacementDuration[] _adPlacementDurations;

		public int SlotCount => _slotCount;
		public float SlotAngle => 360.0f / _slotCount;

		public int EndGameZoneIndex => _endGameZoneIndex;

		public float SpinTargetAngle => _spinTargetAngle;
		public float SpinMinDuration => _spinMinDuration;
		public float SpinMaxDuration => _spinMaxDuration;
		public float SpinSettleDuration => _spinSettleDuration;

		public float SpinSlotOffsetAngle => _spinSlotOffsetAngle;
		public float SpinEdgeBias => _spinEdgeBias;
		public float SpinNearMissChance => _spinNearMissChance;

		public float RewardRevealDelay => _rewardRevealDelay;
		public float BombRevealDelay => _bombRevealDelay;

		public IReadOnlyList<RarityChance> RarityChances => _rarityChances;
		public EItemRarity BaseRarity => _baseRarity;

		public int StartCurrencyAmount => _startCurrencyAmount;

		public int ReviveCostScale => _reviveCostScale;
		public int InitReviveCost => _initReviveCost;

		public IReadOnlyDictionary<EAdPlacement, float> GetAdDurationsByPlacement()
		{
			Dictionary<EAdPlacement, float> durationsByPlacement = new Dictionary<EAdPlacement, float>();

			if (_adPlacementDurations == null)
			{
				return durationsByPlacement;
			}

			foreach (AdPlacementDuration adPlacementDuration in _adPlacementDurations)
			{
				durationsByPlacement[adPlacementDuration.Placement] = adPlacementDuration.Duration;
			}

			return durationsByPlacement;
		}
	}
}
