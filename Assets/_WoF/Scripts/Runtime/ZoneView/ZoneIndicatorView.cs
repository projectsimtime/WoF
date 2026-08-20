using TMPro;
using UnityEngine;
using WoF.Interface;
using Image = UnityEngine.UI.Image;

namespace WoF.ZoneView
{
	/// <summary>
	/// A single milestone chip in the zone indicator bar.
	/// </summary>
	public class ZoneIndicatorView : MonoBehaviour, IStyle<ZoneViewDynamicData>
	{
		[SerializeField]
		private Image _frame;
		[SerializeField]
		private TextMeshProUGUI _label;

		[SerializeField]
		private Image _icon;
		[SerializeField]
		private TextMeshProUGUI _zoneIndexText;

		public void Init(ZoneViewStaticData viewStaticData)
		{
			if (viewStaticData == null)
			{
				return;
			}

			_label.text = viewStaticData.Label;
			_icon.sprite = viewStaticData.Icon;

			_frame.color = viewStaticData.ThemeColor;
			_label.color = viewStaticData.ThemeColor;
			_zoneIndexText.color = viewStaticData.ThemeColor;
		}

		public void ApplyStyle(ZoneViewDynamicData style)
		{
			_zoneIndexText.text = style.ZoneIndex.ToString();

			if (style.Icon != null)
			{
				_icon.sprite = style.Icon;
			}
		}
	}
}
