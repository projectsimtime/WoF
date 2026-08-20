namespace WoF.Session
{
	/// <summary>
	/// The next zone hint offered on the exit screen.
	/// </summary>
	public interface IHintCommands
	{
		void RequestHint();

		void CloseHint();
	}
}
