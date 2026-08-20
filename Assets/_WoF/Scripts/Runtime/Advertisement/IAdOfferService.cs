namespace WoF.Advertisement
{
	/// <summary>
	/// The single call a "watch an ad" button makes. It hands over a placement and knows
	/// nothing about which reward that placement grants or how long the ad runs.
	/// </summary>
	public interface IAdOfferService
	{
		void Play(EAdPlacement placement);
	}
}
