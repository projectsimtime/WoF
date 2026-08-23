using System;
using WoF.UI;

namespace WoF.LosePanel
{
	public class LosePanelGiveUpButton : ButtonController
	{
		public event Action GiveUpClicked;

		protected override void OnButtonClicked()
		{
			GiveUpClicked?.Invoke();
		}
	}
}
