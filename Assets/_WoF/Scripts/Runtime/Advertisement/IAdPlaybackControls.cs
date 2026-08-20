using WoF.Core;

namespace WoF.Advertisement
{
	/// <summary>
	/// Ending an ad, used only by the collect and skip buttons on the ad screen.
	/// </summary>
	public interface IAdPlaybackControls : ISceneService
	{
		void Collect();

		void Skip();
	}
}
