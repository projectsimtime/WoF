namespace WoF
{
	public class RewardSummaryContinueButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.StartNewRun();
		}
	}
}
