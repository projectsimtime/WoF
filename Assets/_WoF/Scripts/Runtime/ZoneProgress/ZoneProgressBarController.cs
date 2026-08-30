using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WoF.Zone;

namespace WoF.ZoneProgress
{
	public class ZoneProgressBarController : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _viewport;

		[SerializeField]
		private RectTransform _content;

		[SerializeField]
		private HorizontalLayoutGroup _layoutGroup;

		[SerializeField]
		private ZoneProgressEntry _zoneProgressEntryPrefab;

		[SerializeField]
		private RectTransform _currentZoneMarker;

		[SerializeField]
		private Image _currentZoneFrame;

		[SerializeField]
		private TextMeshProUGUI _currentZoneText;

		[Tooltip("Use an odd number so the current-zone marker remains centered.")]
		[Min(3)]
		[SerializeField]
		private int _visibleEntryCount = 13;

		[SerializeField]
		private float _slideDuration = 0.3f;

		[SerializeField]
		private float _colorTransitionDuration = 0.45f;

		private List<ZoneProgressEntry> _zoneProgressEntries = new();

		private Sequence _zoneTransitionSequence;

		private ZoneSchedule _zoneSchedule;
		private ZoneDefinition _currentZoneDefinition;

		private int _currentZoneIndex;
		private int _displayedZoneIndex;
		private float _entryStride;

		private int EntryCount => _visibleEntryCount + 1;
		private int PreviousEntryCount => _visibleEntryCount / 2;

		private void OnValidate()
		{
			// We want odd number of entry because so we have only one entry in the center
			if (_visibleEntryCount % 2 == 0)
			{
				--_visibleEntryCount;
			}

			_layoutGroup = GetComponentInChildren<HorizontalLayoutGroup>(true);
			_content = _layoutGroup.GetComponent<RectTransform>();
			_viewport = _content.parent as RectTransform;

			_currentZoneText = GetComponentInChildren<TextMeshProUGUI>(true);
			_currentZoneMarker = _currentZoneText.transform.parent as RectTransform;
		}

		public void Initialize(ZoneSchedule zoneSchedule)
		{
			if (zoneSchedule == null)
			{
				Debug.LogWarning("Zone schedule is null!");
				return;
			}

			_zoneSchedule = zoneSchedule;

			CreateZoneProgressEntries();
			RecalculateEntryLayout();
		}

		public void SetCurrentZone(int zoneIndex, ZoneDefinition zoneDefinition)
		{
			CompleteActiveTransition();

			int previousZoneIndex = _currentZoneIndex;
			_currentZoneIndex = zoneIndex;
			_currentZoneDefinition = zoneDefinition;

			if (previousZoneIndex == 0 || zoneIndex != previousZoneIndex + 1)
			{
				RebuildProgressBar();
				return;
			}

			StartZoneTransition(previousZoneIndex);
		}

		private void OnDestroy()
		{
			_zoneTransitionSequence?.Kill();
		}

		private void OnRectTransformDimensionsChange()
		{
			if (_zoneProgressEntries.Count == 0 || !_viewport || _viewport.rect.width <= 0.0f)
			{
				return;
			}

			CompleteActiveTransition();
			RecalculateEntryLayout();

			if (_currentZoneIndex > 0)
			{
				RebuildProgressBar();
			}
		}

		private void CreateZoneProgressEntries()
		{
			if (_zoneProgressEntries.Count > 0)
			{
				return;
			}

			for (int entryIndex = 0; entryIndex < EntryCount; ++entryIndex)
			{
				_zoneProgressEntries.Add(Instantiate(_zoneProgressEntryPrefab, _content));
			}
		}

		private void RecalculateEntryLayout()
		{
			float totalSpacing = _layoutGroup.spacing * (_visibleEntryCount - 1);
			float entryWidth = (_viewport.rect.width - totalSpacing) / _visibleEntryCount;
			_entryStride = entryWidth + _layoutGroup.spacing;

			foreach (ZoneProgressEntry zoneProgressEntry in _zoneProgressEntries)
			{
				zoneProgressEntry.SetPreferredWidth(entryWidth);
			}

			_content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, EntryCount * entryWidth + (EntryCount - 1) * _layoutGroup.spacing);
			_currentZoneMarker.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, entryWidth);
		}

		private void RebuildProgressBar()
		{
			_content.anchoredPosition = Vector2.zero;

			UpdateZoneProgressEntries(_currentZoneIndex - PreviousEntryCount, _currentZoneIndex);
			UpdateCurrentZoneMarker();
		}

		private void OnZoneTransitionCompleted()
		{
			UpdateZoneProgressEntries(_currentZoneIndex - PreviousEntryCount, _currentZoneIndex);
			_content.anchoredPosition = Vector2.zero;
		}

		private void StartZoneTransition(int previousZoneIndex)
		{
			UpdateZoneProgressEntries(previousZoneIndex - PreviousEntryCount, _currentZoneIndex);
			_currentZoneText.color = _currentZoneDefinition.ZoneIndicatorColor;

			Tween contentSlideTween = _content
				.DOAnchorPosX(-_entryStride, _slideDuration)
				.SetEase(Ease.OutCubic);

			Tween zoneIndexTween = DOTween.To(
				() => _displayedZoneIndex,
				displayedZoneIndex =>
				{
					_displayedZoneIndex = displayedZoneIndex;
					_currentZoneText.text = displayedZoneIndex.ToString();
				},
				_currentZoneIndex,
				_slideDuration)
				.SetEase(Ease.OutCubic);

			Tween currentZoneFrameColorTween = _currentZoneFrame
				.DOColor(_currentZoneDefinition.ThemeColor, _colorTransitionDuration)
				.SetEase(Ease.OutCubic);

			_zoneTransitionSequence = DOTween.Sequence()
				.Append(contentSlideTween)
				.Join(zoneIndexTween)
				.Join(currentZoneFrameColorTween)
				.OnComplete(OnZoneTransitionCompleted)
				.OnKill(() => _zoneTransitionSequence = null);
		}

		private void CompleteActiveTransition()
		{
			_zoneTransitionSequence?.Complete();
		}

		private void UpdateZoneProgressEntries(int firstZoneIndex, int currentZoneIndex)
		{
			for (int entryIndex = 0; entryIndex < _zoneProgressEntries.Count; ++entryIndex)
			{
				UpdateZoneProgressEntry(
					_zoneProgressEntries[entryIndex],
					firstZoneIndex + entryIndex,
					currentZoneIndex);
			}
		}

		private void UpdateZoneProgressEntry(ZoneProgressEntry zoneProgressEntry, int zoneIndex, int currentZoneIndex)
		{
			ZoneDefinition zoneDefinition = _zoneSchedule.GetZoneDefinition(zoneIndex);
			if (!zoneDefinition)
			{
				zoneProgressEntry.SetZoneIndexVisible(false);
				return;
			}

			Color textColor = zoneDefinition.ThemeColor;
			if (zoneIndex < currentZoneIndex)
			{
				textColor *= Color.gray;
			}

			zoneProgressEntry.SetZoneIndexVisuals(zoneIndex, textColor);
			zoneProgressEntry.SetZoneIndexVisible(true);
		}

		private void UpdateCurrentZoneMarker()
		{
			_displayedZoneIndex = _currentZoneIndex;

			_currentZoneFrame.color = _currentZoneDefinition.ThemeColor;

			_currentZoneText.text = _displayedZoneIndex.ToString();
			_currentZoneText.color = _currentZoneDefinition.ZoneIndicatorColor;
		}

	}
}
