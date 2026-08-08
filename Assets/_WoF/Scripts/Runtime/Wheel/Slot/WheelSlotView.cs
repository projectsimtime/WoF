using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WoF.Wheel
{
	public class WheelSlotView : MonoBehaviour
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		private TextMeshProUGUI _amountText;

		private void OnValidate()
		{
			_image = GetComponent<Image>();
			_amountText = GetComponentInChildren<TextMeshProUGUI>();
		}

		public void SetSprite(Sprite sprite)
		{
			_image.sprite = sprite;
		}

		public void SetAmount(int amount)
		{
			if (amount == 0)
			{
				_amountText.gameObject.SetActive(false);
			}
			else
			{
				_amountText.text = $"x{amount}";
				_amountText.gameObject.SetActive(true);
			}
		}
	}
}