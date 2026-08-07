namespace WoF
{
	public class CurrencyBag
	{
		private int _remainingCurrencyAmount;

		public CurrencyBag(int initialCurrencyAmount)
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