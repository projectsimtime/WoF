using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;
using WoF.Zone;

namespace WoF.ZoneView
{
	public class ZoneIndicatorPanelController : MonoBehaviour
	{
		[SerializeField] 
		private ZoneView _zoneViewPrefab;

		[SerializeField] 
		private ZoneTypeData[] _displayedZoneTypes;

		private Dictionary<ZoneTypeData, ZoneView> _zoneViews;

		public void Initialize()
		{
			_zoneViews = new();

			foreach (ZoneTypeData zoneTypeData in _displayedZoneTypes)
			{
				ZoneView zoneView = Instantiate(_zoneViewPrefab, transform);
				zoneView.Init(zoneTypeData.ViewData);

				_zoneViews.Add(zoneTypeData, zoneView);
			}
		}

		public void SetIndicator(ZoneTypeData zoneTypeData, int zoneIndex, RewardDefinition reservedReward)
		{
			if (!_zoneViews.TryGetValue(zoneTypeData, out ZoneView zoneView))
			{
				return;
			}

			if (zoneIndex == int.MaxValue)
			{
				zoneView.gameObject.SetActive(false);
				return;
			}

			zoneView.gameObject.SetActive(true);

			zoneView.ApplyStyle(new ZoneViewDynamicData
			{
				ZoneIndex = zoneIndex,
				Icon = reservedReward ? reservedReward.Sprite : null
			});
		}
	}
}
