using UnityEngine;

namespace WoF
{
	public class LosePanelGiveUpButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnGiveUpButtonClicked();
		}
	}
}