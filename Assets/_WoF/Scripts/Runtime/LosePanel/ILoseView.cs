using WoF.Core;

namespace WoF.LosePanel
{
	/// <summary>
	/// The bomb screen: it shows a price and two offers, it does not decide anything.
	/// </summary>
	public interface ILoseView : ISceneService
	{
		void Show();

		void Hide();

		void ShowReviveCost(int amount);

		void SetReviveEnabled(bool enabled);

		void SetAdOfferVisible(bool visible);
	}
}
