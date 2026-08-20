using WoF.Core;

namespace WoF.Currency
{
	public interface ICurrencyView : ISceneService
	{
		void ShowAmount(int amount);
	}
}
