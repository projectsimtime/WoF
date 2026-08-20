using WoF.Core;
using WoF.Session;
using WoF.UI;

namespace WoF.EndGame
{
	public class EndGameCollectButton : ButtonController
	{
		private IRunCommands _runCommands;

		protected override void OnInject(IServiceResolver services)
		{
			_runCommands = services.Resolve<IRunCommands>();
		}

		protected override void OnButtonClicked()
		{
			_runCommands?.ShowEarnedRewards();
		}
	}
}
