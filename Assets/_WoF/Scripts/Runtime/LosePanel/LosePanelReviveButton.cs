using System;
using UnityEngine;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelReviveButton : ButtonController
	{
		[SerializeField]
		private LosePanelReviveCostText _reviveCostText;

		public event Action ReviveClicked;

		protected override void OnValidate()
		{
			base.OnValidate();

			_reviveCostText = GetComponentInChildren<LosePanelReviveCostText>(true);
		}

		protected override void OnButtonClicked()
		{
			ReviveClicked?.Invoke();
		}

		public void SetReviveCost(int amount)
		{
			_reviveCostText.SetReviveCost(amount);
		}
	}
}
