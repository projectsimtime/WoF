namespace WoF
{
	public class HintPanelCloseButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnHintClosed();
		}
	}
}
