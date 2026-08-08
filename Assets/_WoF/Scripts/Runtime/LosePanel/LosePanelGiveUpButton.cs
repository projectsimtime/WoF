using WoF.UI;

﻿using UnityEngine;

namespace WoF.LosePanel
{
	public class LosePanelGiveUpButton : ButtonController
	{
		protected override void OnButtonClicked()
		{
			_gameSession.OnGiveUpButtonClicked();
		}
	}
}