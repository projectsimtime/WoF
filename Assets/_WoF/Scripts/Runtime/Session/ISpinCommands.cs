using System;

namespace WoF.Session
{
	/// <summary>
	/// What the spin button is allowed to ask for, and nothing more.
	/// </summary>
	public interface ISpinCommands
	{
		bool IsSpinning { get; }

		event Action<bool> SpinningChanged;

		void RequestSpin();
	}
}
