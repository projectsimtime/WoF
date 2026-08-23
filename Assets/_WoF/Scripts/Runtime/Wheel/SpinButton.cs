using System;
using WoF.UI;

namespace WoF.Wheel
{
	public class SpinButton : ButtonController
	{
		public event Action SpinClicked;

		protected override void OnButtonClicked()
		{
			SpinClicked?.Invoke();
		}
	}
}
