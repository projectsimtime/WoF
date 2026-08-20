using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using WoF.Wheel;
using WoF.Zone;
using WoF.ZoneView;

namespace WoF.Session
{
	/// <summary>
	/// Puts a zone on screen: picks the wheel for it, dresses the slots and refreshes the
	/// zone header and milestone bar. It knows nothing about spinning, money or the bomb.
	/// </summary>
	public class ZoneRunner
	{
		private readonly RunState _runState;
		private readonly ZoneCalculator _zoneCalculator;
		private readonly ZoneCatalog _zoneCatalog;
		private readonly ZoneWheelFactory _zoneWheelFactory;
		private readonly MilestoneRewardPlanner _milestoneRewards;
		private readonly RewardService _rewardService;
		private readonly IWheelView _wheelView;
		private readonly IZoneHudView _zoneHudView;
		private readonly IZoneIndicatorView _zoneIndicatorView;

		private readonly List<ZoneMilestoneData> _milestoneBuffer = new List<ZoneMilestoneData>();

		public ZoneRunner(
			RunState runState,
			ZoneCalculator zoneCalculator,
			ZoneCatalog zoneCatalog,
			ZoneWheelFactory zoneWheelFactory,
			MilestoneRewardPlanner milestoneRewards,
			RewardService rewardService,
			IWheelView wheelView,
			IZoneHudView zoneHudView,
			IZoneIndicatorView zoneIndicatorView)
		{
			_runState = runState;
			_zoneCalculator = zoneCalculator;
			_zoneCatalog = zoneCatalog;
			_zoneWheelFactory = zoneWheelFactory;
			_milestoneRewards = milestoneRewards;
			_rewardService = rewardService;
			_wheelView = wheelView;
			_zoneHudView = zoneHudView;
			_zoneIndicatorView = zoneIndicatorView;
		}

		public ZoneWheel CurrentWheel { get; private set; }

		public bool IsRunFinished => _runState.CurrentZone > _zoneCalculator.EndGameZoneIndex;

		/// <summary>
		/// Rolls the special rewards this run will advertise on its milestone zones.
		/// </summary>
		public void PrepareNewRun()
		{
			_zoneWheelFactory.InvalidateCache();

			ZoneTypeData specialRewardZoneType = _zoneCatalog.GetSpecialRewardZoneType();

			_milestoneRewards.Plan(_zoneCalculator.CountZonesOf(specialRewardZoneType));
		}

		public void EnterZone()
		{
			int zoneIndex = _runState.CurrentZone;

			RestoreSlotsFromPreviousZone();

			CurrentWheel = _zoneWheelFactory.GetOrBuild(zoneIndex);

			_runState.ReservedSlotIndex = CurrentWheel.ReservedSlotIndex;
			_runState.IsResumingAfterBomb = false;

			ApplyWheel(CurrentWheel, zoneIndex);
			RefreshIndicators(zoneIndex);
		}

		public RewardDefinition GetReward(int slotIndex)
		{
			if (CurrentWheel.Rewards == null || slotIndex < 0 || slotIndex >= CurrentWheel.Rewards.Count)
			{
				return null;
			}

			return CurrentWheel.Rewards[slotIndex];
		}

		/// <summary>
		/// A revive hides the bomb slot for the extra spin it bought, so the wheel has to be
		/// whole again before the next zone is dressed.
		/// </summary>
		private void RestoreSlotsFromPreviousZone()
		{
			if (_runState.IsResumingAfterBomb)
			{
				_wheelView.SetSlotActive(_runState.ReservedSlotIndex, true);

				return;
			}

			_wheelView.ResetSlots();
		}

		private void ApplyWheel(ZoneWheel zoneWheel, int zoneIndex)
		{
			_wheelView.ApplyWheelType(zoneWheel.WheelType);

			if (zoneWheel.Rewards == null)
			{
				return;
			}

			for (int slotIndex = 0; slotIndex < zoneWheel.Rewards.Count; ++slotIndex)
			{
				RewardDefinition reward = zoneWheel.Rewards[slotIndex];

				_wheelView.ApplySlot(slotIndex, reward, _rewardService.GetAmount(reward, zoneIndex));
			}
		}

		private void RefreshIndicators(int zoneIndex)
		{
			ZoneTypeData currentZoneType = _zoneCalculator.GetZoneType(zoneIndex);

			if (currentZoneType != null && currentZoneType.ViewData != null)
			{
				_zoneHudView.ShowZone(zoneIndex, currentZoneType.ViewData.ThemeColor);
			}

			_milestoneBuffer.Clear();

			foreach (ZoneTypeData zoneType in _zoneCatalog.ZoneTypes)
			{
				if (zoneType == null || !zoneType.ShowsAsMilestone)
				{
					continue;
				}

				int milestoneZoneIndex = _zoneCalculator.GetNextZoneIndexOf(zoneType, zoneIndex);

				_milestoneBuffer.Add(new ZoneMilestoneData
				{
					StaticData = zoneType.ViewData,
					ZoneIndex = milestoneZoneIndex,
					Icon = GetMilestoneIcon(zoneType, milestoneZoneIndex)
				});
			}

			_zoneIndicatorView.ShowMilestones(_milestoneBuffer);
		}

		private Sprite GetMilestoneIcon(ZoneTypeData zoneType, int milestoneZoneIndex)
		{
			if (!zoneType.HasSpecialReward)
			{
				return null;
			}

			RewardDefinition milestoneReward = _milestoneRewards.GetReward(_zoneCalculator.GetOrdinalOf(zoneType, milestoneZoneIndex));

			return milestoneReward == null ? null : milestoneReward.Sprite;
		}
	}
}
