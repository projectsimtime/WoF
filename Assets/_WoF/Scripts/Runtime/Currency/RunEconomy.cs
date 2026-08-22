using WoF.LosePanel;

namespace WoF.Currency
{
	public class RunEconomy
	{
		private int _startCurrencyAmount;

		private CurrencyContainer _currencyContainer;
		private ReviveCostCalculator _reviveCostCalculator;

		public RunEconomy(int startCurrencyAmount, int initReviveCost, int reviveCostScale)
		{
			_startCurrencyAmount = startCurrencyAmount;
			_currencyContainer = new CurrencyContainer(startCurrencyAmount);
			_reviveCostCalculator = new ReviveCostCalculator(initReviveCost, reviveCostScale);
		}

		public void Reset()
		{
			_currencyContainer.Reset(_startCurrencyAmount);
			_reviveCostCalculator.Reset();
		}

		public void PayReviveCost()
		{
			_currencyContainer.AddCurrency(-_reviveCostCalculator.GetReviveCost());
			_reviveCostCalculator.OnRevived();
		}

		public int GetRemainingCurrencyAmount()
		{
			return _currencyContainer.GetRemainingCurrencyAmount();
		}

		public int GetReviveCost()
		{
			return _reviveCostCalculator.GetReviveCost();
		}

		public bool HasEnoughCurrencyForRevive()
		{
			return _currencyContainer.HasEnoughCurrency(GetReviveCost());
		}
	}
}
