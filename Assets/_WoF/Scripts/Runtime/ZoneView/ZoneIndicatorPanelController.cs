using System.Collections.Generic;
using UnityEngine;

namespace WoF.ZoneView
{
	public class ZoneIndicatorPanelController : MonoBehaviour
	{
		[SerializeField]
		private ZoneViewBase _zoneViewPrefab;

		private readonly List<ZoneViewBase> _zoneViews = new List<ZoneViewBase>();

		public void OnNewZone(IReadOnlyList<ZoneIndicatorData> indicators)
		{
			while (_zoneViews.Count < indicators.Count)
			{
				_zoneViews.Add(Instantiate(_zoneViewPrefab, transform));
			}

			for (int i = 0; i < _zoneViews.Count; ++i)
			{
				bool isActive = i < indicators.Count;
				_zoneViews[i].gameObject.SetActive(isActive);

				if (!isActive)
				{
					continue;
				}

				_zoneViews[i].Init(indicators[i].StaticData);
				_zoneViews[i].ApplyStyle(indicators[i].DynamicData);
			}
		}
	}
}
