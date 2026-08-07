using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WoF.Interface;

namespace WoF.Reward
{
	public class RewardCardView : MonoBehaviour, IStyle<EarnedRewardData>
	{
		[SerializeField]
		private Image _rewardIcon;

		[SerializeField]
		private TextMeshProUGUI _labelText;

		[SerializeField]
		private TextMeshProUGUI _amountText;

		public void ApplyStyle(EarnedRewardData style)
		{
			_labelText.text = style.Reward.Label;
			_rewardIcon.sprite = style.Reward.Sprite;
			_amountText.text = $"x{style.Amount}";
		}
	}
}
