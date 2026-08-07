using System;
using DG.Tweening;
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

		private int _cachedAmount;

		private Tween _amountIncreaseTween;

		private void OnValidate()
		{
			_rewardIcon = GetComponentInChildren<Image>();
			_text = GetComponentInChildren<TextMeshProUGUI>();
		}

		private void OnDisable()
		{
			_amountIncreaseTween?.Kill();
		}

		public void ApplyStyle(EarnedRewardData style)
		{
			_rewardIcon.sprite = style.Reward.Sprite;
			
			_amountIncreaseTween?.Kill();
			_amountIncreaseTween = DOTween.To(() => _amount, value => _amount = value, _amount + style.Amount, 1.0f)
				.OnUpdate(() => SetAmount(_amount))
				.SetEase(Ease.OutQuad)
				.OnKill(OnAmountIncreaseKilled);
			
			_cachedAmount = _amount + style.Amount;
		}
		
		private void OnAmountIncreaseKilled()
		{
			_amount = _cachedAmount;
			SetAmount(_cachedAmount);
		}

		private void SetAmount(int amount)
		{
			_text.text = amount.ToString();
		}
	}
}