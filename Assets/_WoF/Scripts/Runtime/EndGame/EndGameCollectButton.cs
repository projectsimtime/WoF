namespace WoF
{
	public class EndGameCollectButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.DisplayEarnedRewards();
		}
	}
}
