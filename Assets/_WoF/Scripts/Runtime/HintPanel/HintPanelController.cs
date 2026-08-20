using UnityEngine;
using WoF.Reward;
using WoF.UI;

namespace WoF.HintPanel
{
	public class HintPanelController : PanelController, IHintView
	{
		[SerializeField]
		private RewardCardView _cardView;

		[SerializeField]
		private HintPanelCloseButton _closeButton;

		private void OnValidate()
		{
			_cardView = GetComponentInChildren<RewardCardView>(true);
			_closeButton = GetComponentInChildren<HintPanelCloseButton>(true);
		}

		public void ShowHint(RewardDefinition reward, int amount)
		{
			_cardView.ApplyStyle(new EarnedRewardData
			{
				Reward = reward,
				Amount = amount
			});
		}
	}
}
