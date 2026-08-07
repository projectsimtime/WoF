using UnityEngine;

namespace WoF
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
