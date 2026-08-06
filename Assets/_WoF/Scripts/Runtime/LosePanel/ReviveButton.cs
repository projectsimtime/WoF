using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
	public class ReviveButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnContinueClicked();
		}
	}
}