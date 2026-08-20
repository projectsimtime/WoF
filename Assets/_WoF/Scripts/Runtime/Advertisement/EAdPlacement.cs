namespace WoF.Advertisement
{
	/// <summary>
	/// Where an ad is offered. Placements are keys into data, so a new offer means a new
	/// reward registration plus a duration row, not a new branch.
	/// </summary>
	public enum EAdPlacement
	{
		Revive = 0,
		ZoneHint = 1
	}
}
