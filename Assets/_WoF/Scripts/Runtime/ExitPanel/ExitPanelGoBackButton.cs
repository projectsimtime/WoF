namespace WoF
{
	public class ExitPanelGoBackButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnGoBackClicked();
		}
	}
}
