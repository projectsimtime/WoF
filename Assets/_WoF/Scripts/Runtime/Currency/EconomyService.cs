namespace WoF.Currency
{
	/// <summary>
	/// The player's purse. It is the only thing that touches the balance, and it keeps the
	/// currency panel in step so no caller has to remember to refresh it.
	/// </summary>
	public class EconomyService
	{
		private readonly CurrencyContainer _currencyContainer;
		private readonly ICurrencyView _currencyView;
		private readonly int _startingAmount;

		public EconomyService(CurrencyContainer currencyContainer, ICurrencyView currencyView, int startingAmount)
		{
			_currencyContainer = currencyContainer;
			_currencyView = currencyView;
			_startingAmount = startingAmount;
		}

		public int Balance => _currencyContainer.GetRemainingCurrencyAmount();

		public bool CanAfford(int amount)
		{
			return _currencyContainer.HasEnoughCurrency(amount);
		}

		public void Spend(int amount)
		{
			_currencyContainer.AddCurrency(-amount);

			Refresh();
		}

		public void ResetForNewRun()
		{
			_currencyContainer.Reset(_startingAmount);

			Refresh();
		}

		private void Refresh()
		{
			_currencyView.ShowAmount(Balance);
		}
	}
}
