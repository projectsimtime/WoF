using WoF.Core;
using WoF.Session;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelGoBackButton : ButtonController
	{
		private IExitCommands _exitCommands;

		protected override void OnInject(IServiceResolver services)
		{
			_exitCommands = services.Resolve<IExitCommands>();
		}

		protected override void OnButtonClicked()
		{
			_exitCommands?.CancelExit();
		}
	}
}
