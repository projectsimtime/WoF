using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WoF.ZoneProgress
{
	public class ZoneProgressEntryView : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _zoneIndexText;
		[SerializeField]
		private LayoutElement _layoutElement;

		public int ZoneIndex { get; private set; }
		public bool HasZone { get; private set; }
		public float LayoutWidth => _layoutElement.preferredWidth;

		private void OnValidate()
		{
			_zoneIndexText = GetComponentInChildren<TextMeshProUGUI>(true);
			_layoutElement = GetComponent<LayoutElement>();
		}

		public void Apply(int zoneIndex, bool hasZone, Color textColor)
		{
			ZoneIndex = zoneIndex;
			HasZone = hasZone;
			_zoneIndexText.text = hasZone ? zoneIndex.ToString() : string.Empty;
			_zoneIndexText.color = textColor;
		}

		public void SetSize(Vector2 itemSize, float layoutWidth)
		{
			RectTransform root = transform as RectTransform;
			root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, layoutWidth);
			root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y);
			_zoneIndexText.rectTransform.sizeDelta = itemSize;
			_layoutElement.preferredWidth = layoutWidth;
			_layoutElement.preferredHeight = itemSize.y;
		}

		public void SetLayoutWidth(float width)
		{
			_layoutElement.preferredWidth = Mathf.Max(0.0f, width);
		}
	}
}
