using System;
using UnityEngine;
using WoF.Reward;
using WoF.UI;

namespace WoF.HintPanel
{
	public class HintPanelController : PanelController
	{
		[SerializeField]
		private RewardCardView _cardView;

		[SerializeField]
		private HintPanelCloseButton _closeButton;

		public event Action Closed;

		private void OnValidate()
		{
			_cardView = GetComponentInChildren<RewardCardView>(true);
			_closeButton = GetComponentInChildren<HintPanelCloseButton>(true);
		}

		private void OnDestroy()
		{
			_closeButton.HintClosed -= OnHintClosed;
		}

		public void Initialize()
		{
			_closeButton.HintClosed += OnHintClosed;
		}

		public void DisplayHint(RewardDefinition reward, int amount)
		{
			_cardView.ApplyStyle(new EarnedRewardData
			{
				Reward = reward,
				Amount = amount
			});
		}

		private void OnHintClosed()
		{
			Closed?.Invoke();
		}
	}
}
