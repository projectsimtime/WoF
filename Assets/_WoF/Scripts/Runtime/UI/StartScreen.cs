using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoF.UI
{
	public class StartScreen : PanelController
	{
		[SerializeField]
		private Button _playButton;

		public event Action PlayRequested;

		private void OnValidate()
		{
			_playButton = GetComponentInChildren<Button>(true);
		}

		private void OnDestroy()
		{
			_playButton.onClick.RemoveListener(OnPlayClicked);
		}

		public void Initialize()
		{
			_playButton.onClick.AddListener(OnPlayClicked);
		}

		private void OnPlayClicked()
		{
			PlayRequested?.Invoke();
		}
	}
}
