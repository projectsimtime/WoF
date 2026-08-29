using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WoF.Zone;

namespace WoF.ZoneProgress
{
	public class ZoneProgressBarView : MonoBehaviour
	{
		[SerializeField]
		[Min(3)]
		private int _visibleCount = 13;
		[SerializeField]
		private Vector2 _itemSize = new(64.0f, 56.0f);
		[SerializeField]
		[Min(0.0f)]
		private float _spacing = 8.0f;
		[SerializeField]
		private Vector2 _padding = new(16.0f, 10.0f);
		[SerializeField]
		[Min(0.0f)]
		private float _transitionDuration = 0.3f;
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float _pastBrightness = 0.45f;
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float _pastAlpha = 0.75f;

		[SerializeField]
		private RectTransform _viewport;
		[SerializeField]
		private RectTransform _content;
		[SerializeField]
		private ZoneProgressEntryView _entryPrefab;
		[SerializeField]
		private Image _currentFill;
		[SerializeField]
		private Image _currentFrame;
		[SerializeField]
		private TextMeshProUGUI _currentPointer;

		private readonly List<ZoneProgressEntryView> _activeEntries = new();
		private ZoneProgressEntryView _spareEntry;
		private ZoneTypeCalculator _zoneTypeCalculator;
		private Tween _shiftTween;
		private int _lastZoneIndex;
		private int _currentZoneIndex = int.MinValue;
		private bool _isInitialized;

		private float ItemStride => _itemSize.x + _spacing;
		private float InnerWidth => _visibleCount * _itemSize.x + (_visibleCount - 1) * _spacing;
		private Vector2 BarSize => new(InnerWidth + _padding.x * 2.0f, _itemSize.y + _padding.y * 2.0f);

		private void OnDisable()
		{
			CompleteShift();
		}

		private void OnValidate()
		{
			_visibleCount = Mathf.Max(3, _visibleCount);
			if (_visibleCount % 2 == 0)
			{
				++_visibleCount;
			}

			_itemSize.x = Mathf.Max(1.0f, _itemSize.x);
			_itemSize.y = Mathf.Max(1.0f, _itemSize.y);
			_padding.x = Mathf.Max(0.0f, _padding.x);
			_padding.y = Mathf.Max(0.0f, _padding.y);
		}

		public void Initialize(ZoneTypeCalculator zoneTypeCalculator, int lastZoneIndex)
		{
			if (_isInitialized)
			{
				return;
			}

			if (zoneTypeCalculator == null)
			{
				throw new ArgumentNullException(nameof(zoneTypeCalculator));
			}

			if (_viewport == null || _content == null || _entryPrefab == null ||
			    _currentFill == null || _currentFrame == null || _currentPointer == null)
			{
				throw new InvalidOperationException("Zone progress bar references are not configured.");
			}

			_zoneTypeCalculator = zoneTypeCalculator;
			_lastZoneIndex = lastZoneIndex;
			ApplyCalculatedLayout();

			for (int i = 0; i < _visibleCount; ++i)
			{
				ZoneProgressEntryView entry = CreateEntry();
				entry.gameObject.SetActive(true);
				_activeEntries.Add(entry);
			}

			_spareEntry = CreateEntry();
			_spareEntry.gameObject.SetActive(false);
			_isInitialized = true;
		}

		public void SetCurrentZone(int zoneIndex)
		{
			if (!_isInitialized)
			{
				throw new InvalidOperationException("Initialize the zone progress bar before setting its current zone.");
			}

			if (zoneIndex < 1 || zoneIndex > _lastZoneIndex)
			{
				throw new ArgumentOutOfRangeException(nameof(zoneIndex));
			}

			CompleteShift();
			int previousZoneIndex = _currentZoneIndex;
			_currentZoneIndex = zoneIndex;

			if (zoneIndex == previousZoneIndex + 1)
			{
				Append(zoneIndex + _visibleCount / 2);
				return;
			}

			Rebuild(zoneIndex);
		}

		private void Rebuild(int currentZoneIndex)
		{
			int halfVisibleCount = _visibleCount / 2;
			for (int i = 0; i < _activeEntries.Count; ++i)
			{
				ZoneProgressEntryView entry = _activeEntries[i];
				entry.transform.SetSiblingIndex(i);
				entry.SetLayoutWidth(ItemStride);
				ApplyEntry(entry, currentZoneIndex - halfVisibleCount + i);
				entry.gameObject.SetActive(true);
			}

			_spareEntry.gameObject.SetActive(false);
			ApplyCurrentSelector();
			LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
		}

		private void Append(int zoneIndex)
		{
			ZoneProgressEntryView outgoingEntry = _activeEntries[0];
			ZoneProgressEntryView incomingEntry = _spareEntry;

			incomingEntry.transform.SetAsLastSibling();
			ApplyEntry(incomingEntry, zoneIndex);
			incomingEntry.SetLayoutWidth(0.0f);
			incomingEntry.gameObject.SetActive(true);
			RefreshStyles();
			ApplyCurrentSelector();
			LayoutRebuilder.ForceRebuildLayoutImmediate(_content);

			_shiftTween = DOTween.To(
				() => incomingEntry.LayoutWidth,
				incomingEntry.SetLayoutWidth,
				ItemStride,
				_transitionDuration)
				.SetEase(Ease.InOutCubic)
				.OnComplete(() => CompleteAppend(outgoingEntry, incomingEntry));
		}

		private void CompleteAppend(ZoneProgressEntryView outgoingEntry, ZoneProgressEntryView incomingEntry)
		{
			_shiftTween = null;
			_activeEntries.RemoveAt(0);
			_activeEntries.Add(incomingEntry);
			outgoingEntry.gameObject.SetActive(false);
			outgoingEntry.SetLayoutWidth(ItemStride);
			_spareEntry = outgoingEntry;
			LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
		}

		private ZoneProgressEntryView CreateEntry()
		{
			ZoneProgressEntryView entry = Instantiate(_entryPrefab, _content, false);
			entry.SetSize(_itemSize, ItemStride);
			return entry;
		}

		private void ApplyCalculatedLayout()
		{
			RectTransform root = transform as RectTransform;
			root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BarSize.x);
			root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BarSize.y);
			_viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, InnerWidth);
			_viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _itemSize.y);

			Vector2 selectorSize = new(ItemStride, BarSize.y - 2.0f);
			_currentFill.rectTransform.sizeDelta = selectorSize;
			_currentFrame.rectTransform.sizeDelta = selectorSize;
		}

		private void RefreshStyles()
		{
			foreach (ZoneProgressEntryView entry in _activeEntries)
			{
				ApplyEntry(entry, entry.ZoneIndex);
			}

			if (_spareEntry.gameObject.activeSelf)
			{
				ApplyEntry(_spareEntry, _spareEntry.ZoneIndex);
			}
		}

		private void ApplyEntry(ZoneProgressEntryView entry, int zoneIndex)
		{
			bool hasZone = zoneIndex >= 1 && zoneIndex <= _lastZoneIndex;
			Color color = hasZone ? GetThemeColor(zoneIndex) : Color.clear;

			if (hasZone && zoneIndex < _currentZoneIndex)
			{
				color.r *= _pastBrightness;
				color.g *= _pastBrightness;
				color.b *= _pastBrightness;
				color.a *= _pastAlpha;
			}

			entry.Apply(zoneIndex, hasZone, color);
		}

		private void ApplyCurrentSelector()
		{
			Color themeColor = GetThemeColor(_currentZoneIndex);
			Color fillColor = Color.Lerp(Color.black, themeColor, 0.55f);
			fillColor.a = 0.92f;

			_currentFill.color = fillColor;
			_currentFrame.color = themeColor;
			_currentPointer.color = themeColor;
		}

		private Color GetThemeColor(int zoneIndex)
		{
			ZoneTypeData zoneTypeData = _zoneTypeCalculator.GetZoneTypeDataFromIndex(zoneIndex);
			return zoneTypeData != null ? zoneTypeData.ViewData.ThemeColor : Color.white;
		}

		private void CompleteShift()
		{
			if (_shiftTween == null)
			{
				return;
			}

			Tween shiftTween = _shiftTween;
			_shiftTween = null;
			shiftTween.Complete(true);
		}
	}
}
