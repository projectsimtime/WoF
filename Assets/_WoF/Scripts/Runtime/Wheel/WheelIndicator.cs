using System;
using UnityEngine;
using UnityEngine.UI;
using WoF.Interface;

namespace WoF.Wheel
{
	public class WheelIndicator : MonoBehaviour, IStyle<WheelType>
	{
		[SerializeField]
		private Image _image;
		
		private void OnValidate()
		{
			_image = GetComponent<Image>();
		}
		
		public void ApplyStyle(WheelType style)
		{
			_image.sprite = style.IndicatorSprite;
		}
	}
}