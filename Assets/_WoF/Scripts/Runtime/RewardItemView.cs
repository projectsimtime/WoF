using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WoF.Interface;
using WoF.Reward;

namespace WoF
{
	public struct EarnedRewardData
	{
		public RewardDefinition Reward;
		public int Amount;
	}
	
	public class RewardItemView : MonoBehaviour, IStyle<EarnedRewardData>
	{
		[SerializeField] 
		private Image _rewardIcon;

		[SerializeField] 
		private TextMeshProUGUI _text;

		private int _amount;

		private void OnValidate()
		{
			_rewardIcon = GetComponentInChildren<Image>();
			_text = GetComponentInChildren<TextMeshProUGUI>();
		}

		public void ApplyStyle(EarnedRewardData style)
		{
			_rewardIcon.sprite = style.Reward.Sprite;
			_amount += style.Amount;
			
			_text.text = _amount.ToString();
		}
	}
}