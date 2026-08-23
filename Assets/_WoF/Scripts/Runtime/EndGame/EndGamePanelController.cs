using System;
using UnityEngine;
using WoF.UI;

namespace WoF.EndGame
{
	public class EndGamePanelController : PanelController
	{
		[SerializeField]
		private EndGameCollectButton _endGameCollectButton;

		public event Action CollectRequested;

		private void OnValidate()
		{
			_endGameCollectButton = GetComponentInChildren<EndGameCollectButton>(true);
		}

		private void OnDestroy()
		{
			_endGameCollectButton.CollectClicked -= OnCollectClicked;
		}

		public void Initialize()
		{
			_endGameCollectButton.CollectClicked += OnCollectClicked;
		}

		private void OnCollectClicked()
		{
			CollectRequested?.Invoke();
		}
	}
}
