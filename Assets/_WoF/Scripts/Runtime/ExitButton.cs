namespace WoF
{
	public class ExitButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnExitClicked();
		}
	}
}
