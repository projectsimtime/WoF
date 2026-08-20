namespace WoF.Session
{
	/// <summary>
	/// Resolved by development tooling only, so shortcuts never leak into gameplay code.
	/// </summary>
	public interface IDebugRunCommands
	{
		void JumpToZone(int zoneIndex);
	}
}
