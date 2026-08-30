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
		private ZoneDefinition[] _displayedZoneDefinitions;

		private Dictionary<ZoneDefinition, ZoneView> _zoneViewByZoneDefinition;

		public void Initialize()
		{
			_zoneViewByZoneDefinition = new();

			foreach (ZoneDefinition zoneDefinition in _displayedZoneDefinitions)
			{
				ZoneView zoneView = Instantiate(_zoneViewPrefab, transform);
				zoneView.Init(zoneDefinition);

				_zoneViewByZoneDefinition.Add(zoneDefinition, zoneView);
			}
		}

		public void SetIndicator(ZoneDefinition zoneDefinition, int zoneIndex, RewardDefinition reservedReward)
		{
			if (!_zoneViewByZoneDefinition.TryGetValue(zoneDefinition, out ZoneView zoneView))
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
