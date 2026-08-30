using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WoF.ZoneProgress
{
	public class ZoneProgressEntry : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _zoneIndexText;

		[SerializeField]
		private LayoutElement _layoutElement;

		private void OnValidate()
		{
			_zoneIndexText = GetComponent<TextMeshProUGUI>();
			_layoutElement = GetComponent<LayoutElement>();
		}

		public void SetPreferredWidth(float preferredWidth)
		{
			_layoutElement.preferredWidth = preferredWidth;
		}

		public void SetZoneIndexVisuals(int zoneIndex, Color textColor)
		{
			_zoneIndexText.text = zoneIndex.ToString();
			_zoneIndexText.color = textColor;
		}

		public void SetZoneIndexVisible(bool isVisible)
		{
			_zoneIndexText.enabled = isVisible;
		}
	}
}
