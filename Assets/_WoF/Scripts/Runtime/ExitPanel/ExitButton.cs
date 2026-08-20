using WoF.Core;
using WoF.Session;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitButton : ButtonController
	{
		private IExitCommands _exitCommands;

		protected override void OnInject(IServiceResolver services)
		{
			_exitCommands = services.Resolve<IExitCommands>();
			_exitCommands.ExitAvailabilityChanged += OnExitAvailabilityChanged;

			OnExitAvailabilityChanged(_exitCommands.IsExitAvailable);
		}

		private void OnDestroy()
		{
			if (_exitCommands != null)
			{
				_exitCommands.ExitAvailabilityChanged -= OnExitAvailabilityChanged;
			}
		}

		protected override void OnButtonClicked()
		{
			_exitCommands?.RequestExit();
		}

		private void OnExitAvailabilityChanged(bool available)
		{
			SetInteractable(available);
		}
	}
}
