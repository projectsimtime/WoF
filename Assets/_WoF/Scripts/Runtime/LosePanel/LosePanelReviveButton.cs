using UnityEngine;
using WoF.Core;
using WoF.Session;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelReviveButton : ButtonController
	{
		[SerializeField]
		private LosePanelReviveCostText _reviveCostText;

		private IReviveCommands _reviveCommands;

		protected override void OnValidate()
		{
			base.OnValidate();

			_reviveCostText = GetComponentInChildren<LosePanelReviveCostText>(true);
		}

		protected override void OnInject(IServiceResolver services)
		{
			_reviveCommands = services.Resolve<IReviveCommands>();
		}

		protected override void OnButtonClicked()
		{
			_reviveCommands?.ReviveWithCurrency();
		}

		public void SetReviveCost(int amount)
		{
			_reviveCostText.SetReviveCost(amount);
		}
	}
}
