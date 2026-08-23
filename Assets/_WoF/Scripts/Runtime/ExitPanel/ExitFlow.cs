using System;
using UnityEngine;
using WoF.HintPanel;
using WoF.Reward;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitFlow : PanelController
	{
		[SerializeField]
		private ExitPanelCollectRewardButton exitPanelCollectRewardButton;
		[SerializeField]
		private ExitPanelGoBackButton exitPanelGoBackButton;
		[SerializeField]
		private ExitPanelSeeAdButton _exitPanelSeeAdButton;
		[SerializeField]
		private ExitButton _exitButton;
		[SerializeField]
		private HintPanelController _hintPanelController;

		public event Action CollectRequested;
		public event Action HintRequested;

		private void OnValidate()
		{
			exitPanelCollectRewardButton = GetComponentInChildren<ExitPanelCollectRewardButton>();
			exitPanelGoBackButton = GetComponentInChildren<ExitPanelGoBackButton>();
		}

		private void OnDestroy()
		{
			exitPanelCollectRewardButton.CollectClicked -= OnCollectRewardClicked;
			exitPanelGoBackButton.GoBackClicked -= OnGoBackClicked;
			_exitPanelSeeAdButton.HintRewardGranted -= OnHintRewardGranted;
			_exitButton.ExitClicked -= OnExitClicked;
			_hintPanelController.Closed -= OnHintClosed;
		}

		public void Initialize()
		{
			exitPanelCollectRewardButton.CollectClicked += OnCollectRewardClicked;
			exitPanelGoBackButton.GoBackClicked += OnGoBackClicked;
			_exitPanelSeeAdButton.HintRewardGranted += OnHintRewardGranted;
			_exitButton.ExitClicked += OnExitClicked;

			_hintPanelController.Initialize();
			_hintPanelController.Closed += OnHintClosed;
		}

		public void ResetRun()
		{
			_hintPanelController.Hide();
		}

		public void SetExitAvailability(bool available)
		{
			_exitButton.SetButtonInteractable(available);
		}

		public void OnHintClicked(RewardDefinition reward, int amount)
		{
			Hide();
			_hintPanelController.Show();
			_hintPanelController.DisplayHint(reward, amount);
		}

		private void OnExitClicked()
		{
			Show();
		}

		private void OnGoBackClicked()
		{
			Hide();
		}

		private void OnCollectRewardClicked()
		{
			Hide();
			CollectRequested?.Invoke();
		}

		private void OnHintRewardGranted()
		{
			HintRequested?.Invoke();
		}

		private void OnHintClosed()
		{
			_hintPanelController.Hide();
			Show();
		}
	}
}
