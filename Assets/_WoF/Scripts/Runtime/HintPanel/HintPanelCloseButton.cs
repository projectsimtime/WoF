using WoF.UI;

namespace WoF.HintPanel
{
	public class HintPanelCloseButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnHintClosed();
		}
	}
}
