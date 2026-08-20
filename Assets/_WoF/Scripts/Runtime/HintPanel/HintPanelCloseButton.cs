using WoF.Core;
using WoF.Session;
using WoF.UI;

namespace WoF.HintPanel
{
	public class HintPanelCloseButton : ButtonController
	{
		private IHintCommands _hintCommands;

		protected override void OnInject(IServiceResolver services)
		{
			_hintCommands = services.Resolve<IHintCommands>();
		}

		protected override void OnButtonClicked()
		{
			_hintCommands?.CloseHint();
		}
	}
}
