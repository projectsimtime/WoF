using UnityEngine;

namespace WoF.Currency
{
	public class CurrencyPanelController : MonoBehaviour, ICurrencyView
	{
		[SerializeField]
		private CurrencyPanelValue _currency;

		private void OnValidate()
		{
			_currency = GetComponentInChildren<CurrencyPanelValue>(true);
		}

		public void ShowAmount(int amount)
		{
			_currency.SetCurrencyAmount(amount);
		}
	}
}
