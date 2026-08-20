namespace WoF.Session
{
	/// <summary>
	/// The three ways out of the bomb screen.
	/// </summary>
	public interface IReviveCommands
	{
		void ReviveWithCurrency();

		void ReviveWithAd();

		void GiveUp();
	}
}
