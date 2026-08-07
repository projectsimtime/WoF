namespace WoF
{
	public class ExitPanelCollectRewardButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnCollectRewardClicked();
		}
	}
}
