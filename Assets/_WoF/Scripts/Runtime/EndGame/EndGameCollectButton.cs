using WoF.UI;

namespace WoF.EndGame
{
	public class EndGameCollectButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.DisplayEarnedRewards();
		}
	}
}
