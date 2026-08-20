namespace WoF.Session
{
	/// <summary>
	/// Starting over and cashing out, used by the end game and reward summary buttons.
	/// </summary>
	public interface IRunCommands
	{
		void StartNewRun();

		void ShowEarnedRewards();
	}
}
