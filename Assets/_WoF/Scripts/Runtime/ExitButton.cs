namespace WoF
{
	public class ExitButton : ButtonController
	{
		protected override void OnEnable()
		{
			base.OnEnable();

			_gameSession.ExitAvailabilityChanged += OnExitAvailabilityChanged;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			_gameSession.ExitAvailabilityChanged -= OnExitAvailabilityChanged;
		}

		protected override void OnButtonClicked()
		{
			_gameSession.OnExitClicked();
		}

		private void OnExitAvailabilityChanged(bool available)
		{
			SetButtonInteractable(available);
		}
	}
}
