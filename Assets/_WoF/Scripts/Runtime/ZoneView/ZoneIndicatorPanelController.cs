using System;
using UnityEngine;

namespace WoF
{
	public class ZoneIndicatorPanelController : MonoBehaviour
	{
		[SerializeField] 
		private ZoneViewBase _zoneViewPrefab;

		[SerializeField] 
		private ZoneViewStaticData _superZoneStaticData;
		[SerializeField] 
		private ZoneViewStaticData _safeZoneStaticData;

		private ZoneViewBase _superZoneView;
		private ZoneViewBase _safeZoneView;
		private void Awake()
		{
			_superZoneView = Instantiate(_zoneViewPrefab, transform);
			_safeZoneView = Instantiate(_zoneViewPrefab, transform);

			_superZoneView.Init(_superZoneStaticData);
			_safeZoneView.Init(_safeZoneStaticData);
		}

		public void OnNewLevel(int nextSafeZoneIndex, int nextSuperZoneIndex, Sprite superZoneIcon)
		{
			_superZoneView.ApplyStyle(new ZoneViewDynamicData
			{
				ZoneIndex = nextSuperZoneIndex,
				Icon = superZoneIcon
			});

			_safeZoneView.ApplyStyle(new ZoneViewDynamicData
			{
				ZoneIndex = nextSafeZoneIndex
			});
		}
	}
}