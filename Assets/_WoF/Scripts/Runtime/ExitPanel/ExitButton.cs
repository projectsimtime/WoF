using System;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitButton : ButtonController
	{
		public event Action ExitClicked;

		protected override void OnButtonClicked()
		{
			ExitClicked?.Invoke();
		}
	}
}
