using System;

namespace WoF.Session
{
	/// <summary>
	/// Leaving the run with what has been collected so far.
	/// </summary>
	public interface IExitCommands
	{
		bool IsExitAvailable { get; }

		event Action<bool> ExitAvailabilityChanged;

		void RequestExit();

		void CancelExit();

		void CollectAndExit();
	}
}
