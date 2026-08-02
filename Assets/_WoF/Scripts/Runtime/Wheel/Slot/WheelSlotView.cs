using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
	public class WheelSlotView : MonoBehaviour
	{
		[SerializeField]
		private Image _image;

		private void OnValidate()
		{
			_image = GetComponent<Image>();
		}

		public void SetSprite(Sprite sprite)
		{
			_image.sprite = sprite;
		}
	}
}