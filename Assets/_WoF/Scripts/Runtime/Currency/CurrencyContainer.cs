namespace WoF.Currency
{
	public class CurrencyContainer
	{
		private int _remainingCurrencyAmount;

		public CurrencyContainer(int initialCurrencyAmount)
		{
			_remainingCurrencyAmount = initialCurrencyAmount;
		}

		public void AddCurrency(int amount)
		{
			_remainingCurrencyAmount += amount;
		}

		public bool HasEnoughCurrency(int amount)
		{
			return _remainingCurrencyAmount >= amount;
		}

		public int GetRemainingCurrencyAmount()
		{
			return _remainingCurrencyAmount;
		}

		public void Reset(int initialCurrencyAmount)
		{
			_remainingCurrencyAmount = initialCurrencyAmount;
		}
	}
}