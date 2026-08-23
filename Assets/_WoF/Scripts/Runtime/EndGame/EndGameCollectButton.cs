using System;
using WoF.UI;

namespace WoF.EndGame
{
	public class EndGameCollectButton : ButtonController
	{
		public event Action CollectClicked;

		protected override void OnButtonClicked()
		{
			CollectClicked?.Invoke();
		}
	}
}
