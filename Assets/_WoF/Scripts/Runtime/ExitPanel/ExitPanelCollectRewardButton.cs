using System;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelCollectRewardButton : ButtonController
	{
		public event Action CollectClicked;

		protected override void OnButtonClicked()
		{
			CollectClicked?.Invoke();
		}
	}
}
