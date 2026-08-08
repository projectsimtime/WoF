using WoF.UI;

namespace WoF.RewardSummary
{
	public class RewardSummaryContinueButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.StartNewRun();
		}
	}
}
