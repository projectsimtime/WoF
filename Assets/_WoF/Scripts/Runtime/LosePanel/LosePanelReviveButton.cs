using UnityEngine;
using UnityEngine.UI;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelReviveButton : ButtonController
	{
		[SerializeField] 
		private LosePanelReviveCostText _reviveCostText;

		protected override void OnValidate()
		{
			base.OnValidate();

			_reviveCostText = GetComponentInChildren<LosePanelReviveCostText>(true);
		}

		protected override void OnButtonClicked()
		{
			_gameSession.OnReviveWithCurrency();
		}

		public void SetReviveCost(int amount)
		{
			_reviveCostText.SetReviveCost(amount);
		}
	}
}