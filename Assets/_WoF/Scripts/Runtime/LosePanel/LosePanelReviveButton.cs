using UnityEngine;
using UnityEngine.UI;

namespace WoF
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
			_gameSession.OnReviveButtonClicked();
		}

		public void SetReviveCost(int amount)
		{
			_reviveCostText.SetReviveCost(amount);
		}
	}
}