using System;
using DG.Tweening;
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
			_amountText = GetComponentInChildren<TextMeshProUGUI>(true);
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

		public Tween PlayFlash(float duration)
		{
			_image.DOKill();
			Color originalColor = _image.color;

			return _image.DOColor(new Color(1.0f, 0.25f, 0.1f, 1.0f), duration * 0.5f)
				.SetLoops(2, LoopType.Yoyo)
				.SetEase(Ease.Linear)
				.OnComplete(() => _image.color = originalColor);
		}
	}
}
