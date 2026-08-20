using WoF.Core;

namespace WoF.Advertisement
{
	/// <summary>
	/// Starting an ad. Split from <see cref="IAdPlaybackControls"/> so the buttons inside the
	/// ad screen cannot start one and the offers outside it cannot end one.
	/// </summary>
	public interface IAdPlayer : ISceneService
	{
		void Play(IAdReward reward, float duration);
	}
}
