namespace WoF
{
	public class GoBackButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnGoBackClicked();
		}
	}
}
