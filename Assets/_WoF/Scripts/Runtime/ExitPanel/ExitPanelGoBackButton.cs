using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelGoBackButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnGoBackClicked();
		}
	}
}
