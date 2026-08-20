using System.Collections.Generic;
using UnityEngine;

namespace WoF.ZoneView
{
	public class ZoneIndicatorPanelController : MonoBehaviour, IZoneIndicatorView
	{
		[SerializeField]
		private ZoneIndicatorView _zoneViewPrefab;

		private readonly List<ZoneIndicatorView> _milestoneViews = new List<ZoneIndicatorView>();

		public void ShowMilestones(IReadOnlyList<ZoneMilestoneData> milestones)
		{
			if (milestones == null)
			{
				return;
			}

			for (int i = 0; i < milestones.Count; ++i)
			{
				ZoneMilestoneData milestone = milestones[i];

				GetOrCreateView(i, milestone).ApplyStyle(new ZoneViewDynamicData
				{
					ZoneIndex = milestone.ZoneIndex,
					Icon = milestone.Icon
				});
			}

			for (int i = milestones.Count; i < _milestoneViews.Count; ++i)
			{
				_milestoneViews[i].gameObject.SetActive(false);
			}
		}

		private ZoneIndicatorView GetOrCreateView(int index, ZoneMilestoneData milestone)
		{
			while (_milestoneViews.Count <= index)
			{
				_milestoneViews.Add(Instantiate(_zoneViewPrefab, transform));
			}

			ZoneIndicatorView milestoneView = _milestoneViews[index];

			milestoneView.gameObject.SetActive(true);
			milestoneView.Init(milestone.StaticData);

			return milestoneView;
		}
	}
}
