using System;
using WoF.UI;

namespace WoF.HintPanel
{
	public class HintPanelCloseButton : ButtonController
	{
		public event Action HintClosed;

		protected override void OnButtonClicked()
		{
			HintClosed?.Invoke();
		}
	}
}
