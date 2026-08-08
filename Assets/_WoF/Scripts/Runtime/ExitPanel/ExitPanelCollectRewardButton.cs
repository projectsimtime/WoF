using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelCollectRewardButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnCollectRewardClicked();
		}
	}
}
