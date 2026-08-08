using UnityEngine;
using WoF.UI;

namespace WoF.EndGame
{
	public class EndGamePanelController : PanelController
	{
		[SerializeField]
		private EndGameCollectButton _endGameCollectButton;

		private void OnValidate()
		{
			_endGameCollectButton = GetComponentInChildren<EndGameCollectButton>(true);
		}
	}
}
