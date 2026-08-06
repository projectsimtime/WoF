using UnityEngine;

namespace WoF
{
	public class GiveUpButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnGiveUpButtonClicked();
		}
	}
}