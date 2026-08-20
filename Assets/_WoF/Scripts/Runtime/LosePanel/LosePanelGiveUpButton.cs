using WoF.Core;
using WoF.Session;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelGiveUpButton : ButtonController
	{
		private IReviveCommands _reviveCommands;

		protected override void OnInject(IServiceResolver services)
		{
			_reviveCommands = services.Resolve<IReviveCommands>();
		}

		protected override void OnButtonClicked()
		{
			_reviveCommands?.GiveUp();
		}
	}
}
