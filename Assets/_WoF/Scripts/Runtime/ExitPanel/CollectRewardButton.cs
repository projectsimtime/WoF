namespace WoF
{
	public class CollectRewardButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnCollectRewardClicked();
		}
	}
}
