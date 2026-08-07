using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoF
{
	public class CurrencyPanelController : MonoBehaviour
	{
		[SerializeField] private CurrencyPanelValue _currency;

		private void OnValidate()
		{
			_currency = GetComponentInChildren<CurrencyPanelValue>(true);
		}

		public void SetCurrencyAmount(int amount)
		{
			_currency.SetCurrencyAmount(amount);
		}
	}
}