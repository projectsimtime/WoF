using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using WoF.Zone;
using Random = UnityEngine.Random;

namespace WoF.Wheel
{
	public class WheelFlow : MonoBehaviour
	{
		[SerializeField]
		private WheelSpin _wheelSpin;
		[SerializeField]
		private WheelIndicator _wheelIndicator;
		[SerializeField]
		private SpinButton _spinButton;

		private GameSettings _gameSettings;
		private RewardAmountMap _rewardAmountMap;
		private ZoneWheelBuilder _zoneWheelBuilder;
		private SpinAngleCalculator _spinAngleCalculator;

		private List<RewardDefinition> _rewardDefinitions = new();
		private RewardDefinition _earnedReward;

		private ZoneWheelData _nextZoneWheel;
		private bool _hasNextZoneWheel;

		private int _reservedSlotIndex;
		private bool _isContinueAfterBomb;

		public event Action<RewardDefinition> SpinCompleted;
		public event Action<bool> SpinStateChanged;

		private void OnValidate()
		{
			_wheelSpin = GetComponentInChildren<WheelSpin>(true);
			_wheelIndicator = GetComponentInChildren<WheelIndicator>(true);
		}

		private void OnDestroy()
		{
			_spinButton.SpinClicked -= OnSpinClicked;
		}

		public void Initialize(ZoneRules zoneRules, ZoneTypeCalculator zoneTypeCalculator, GameSettings gameSettings, RewardAmountMap rewardAmountMap)
		{
			_gameSettings = gameSettings;
			_rewardAmountMap = rewardAmountMap;
			_zoneWheelBuilder = new ZoneWheelBuilder(zoneRules, zoneTypeCalculator, gameSettings.SlotCount);
			_spinAngleCalculator = new SpinAngleCalculator(gameSettings.SlotCount, gameSettings.SpinTargetAngle, gameSettings.SpinSlotOffsetAngle, gameSettings.SpinEdgeBias, gameSettings.SpinNearMissChance);

			_spinButton.SpinClicked += OnSpinClicked;
		}

		public void ResetRun()
		{
			_earnedReward = null;
			_rewardDefinitions.Clear();
			_zoneWheelBuilder.Reset();
			_hasNextZoneWheel = false;
		}

		public void EnterZone(int zoneIndex)
		{
			_wheelSpin.SetRotation(Vector3.zero);

			ZoneWheelData zoneWheel = GetZoneWheel(zoneIndex);
			
			ApplyZoneWheel(zoneWheel, zoneIndex);

			if (_isContinueAfterBomb)
			{
				_wheelSpin.SetActiveWheelSlot(_reservedSlotIndex, true);
				_isContinueAfterBomb = false;
			}
			
			_reservedSlotIndex = zoneWheel.ReservedSlotIndex;
			_rewardDefinitions = zoneWheel.Rewards;
		}

		private void OnSpinClicked()
		{
			SetWheelSpinning(true);

			float angle = _spinAngleCalculator.CalculateTargetAngle(_reservedSlotIndex, _isContinueAfterBomb, out var earnedRewardIndex);
			float duration = Random.Range(_gameSettings.SpinMinDuration, _gameSettings.SpinMaxDuration);

			var tween = _wheelSpin.PlaySpin(angle, duration);
			tween.onComplete += OnSpinComplete;

			_earnedReward = _rewardDefinitions[earnedRewardIndex];
		}

		public void OnRevived()
		{
			_isContinueAfterBomb = true;
			_wheelSpin.SetActiveWheelSlot(_reservedSlotIndex, false);
		}

		public RewardDefinition GetBestPreviewReward(int zoneIndex)
		{
			List<RewardDefinition> previewRewards = GetPreviewRewards(zoneIndex);
			RewardDefinition bestReward = null;

			foreach (RewardDefinition reward in previewRewards)
			{
				if (reward.Kind == EItemKind.Bomb)
				{
					continue;
				}

				if (bestReward == null || reward.Rarity.Rank > bestReward.Rarity.Rank)
				{
					bestReward = reward;
				}
			}

			return bestReward;
		}

		private List<RewardDefinition> GetPreviewRewards(int zoneIndex)
		{
			if (!_hasNextZoneWheel)
			{
				_nextZoneWheel = _zoneWheelBuilder.Build(zoneIndex);
				_hasNextZoneWheel = true;
			}

			return _nextZoneWheel.Rewards;
		}

		public RewardDefinition GetReservedReward(int zoneIndex)
		{
			return _zoneWheelBuilder.GetReservedReward(zoneIndex);
		}

		private ZoneWheelData GetZoneWheel(int zoneIndex)
		{
			if (_hasNextZoneWheel)
			{
				_hasNextZoneWheel = false;
				return _nextZoneWheel;
			}

			return _zoneWheelBuilder.Build(zoneIndex);
		}

		private void ApplyZoneWheel(ZoneWheelData zoneWheel, int zoneIndex)
		{
			_wheelSpin.ApplyStyle(zoneWheel.WheelType);
			_wheelIndicator.ApplyStyle(zoneWheel.WheelType);

			for (int i = 0; i < zoneWheel.Rewards.Count; ++i)
			{
				RewardDefinition reward = zoneWheel.Rewards[i];
				_wheelSpin.ApplySlotView(i, reward, _rewardAmountMap.GetAmountByKind(reward, zoneIndex));
			}
		}

		private void SetWheelSpinning(bool isSpinning)
		{
			_spinButton.SetButtonInteractable(!isSpinning);
			SpinStateChanged?.Invoke(isSpinning);
		}

		private void OnSpinComplete()
		{
			SetWheelSpinning(false);
			SpinCompleted?.Invoke(_earnedReward);
		}
	}
}
