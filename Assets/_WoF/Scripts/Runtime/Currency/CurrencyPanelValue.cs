using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WoF.Currency
{
	public class CurrencyPanelValue : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;

		private void OnValidate()
		{
			_text = GetComponent<TextMeshProUGUI>();
		}

		public void SetCurrencyAmount(int amount)
		{
			_text.text = amount.ToString();
		}
	}
}