using System;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelGoBackButton : ButtonController
	{
		public event Action GoBackClicked;

		protected override void OnButtonClicked()
		{
			GoBackClicked?.Invoke();
		}
	}
}
