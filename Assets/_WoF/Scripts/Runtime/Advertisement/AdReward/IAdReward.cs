namespace WoF.Advertisement
{
	/// <summary>
	/// What the player gets for sitting through an ad. Implementations depend on the single
	/// command they trigger, never on the whole game.
	/// </summary>
	public interface IAdReward
	{
		void Grant();
	}
}
