using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
	public class LosePanelReviveButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnContinueClicked();
		}
	}
}