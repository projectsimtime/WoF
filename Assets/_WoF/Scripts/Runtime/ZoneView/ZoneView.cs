using System;
using TMPro;
using UnityEngine;
using WoF.Interface;
using WoF.Zone;
using Image = UnityEngine.UI.Image;

namespace WoF.ZoneView
{
	public class ZoneView : MonoBehaviour, IStyle<ZoneViewDynamicData>
	{
		
		[SerializeField]
		private Image _frame;
		[SerializeField]
		private TextMeshProUGUI _label;
		
		[SerializeField]
		private Image _icon;
		[SerializeField]
		private TextMeshProUGUI _zoneIndexText;

		public void Init(ZoneTypeData zoneTypeData)
		{
			_label.text = zoneTypeData.Label;
			_icon.sprite = zoneTypeData.Icon;
			
			_frame.color = zoneTypeData.ThemeColor;
			_label.color = zoneTypeData.ThemeColor;
			_zoneIndexText.color = zoneTypeData.ThemeColor;
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
