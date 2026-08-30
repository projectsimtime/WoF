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

		public void Init(ZoneDefinition zoneDefinition)
		{
			_label.text = zoneDefinition.Label;
			_icon.sprite = zoneDefinition.Icon;
			
			_frame.color = zoneDefinition.ThemeColor;
			_label.color = zoneDefinition.ThemeColor;
			_zoneIndexText.color = zoneDefinition.ThemeColor;
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
