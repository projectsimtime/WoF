using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WoF.Interface;
using WoF.Reward;

namespace WoF.Reward
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
		[SerializeField]
		private CanvasGroup _canvasGroup;

		private int _amount;

		private int _cachedAmount;

		private Tween _amountIncreaseTween;

		public RectTransform RectTransform => transform as RectTransform;
		public RectTransform RevealTarget => _rewardIcon.rectTransform;

		private void OnValidate()
		{
			_rewardIcon = GetComponentInChildren<Image>(true);
			_text = GetComponentInChildren<TextMeshProUGUI>(true);
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void OnDisable()
		{
			_amountIncreaseTween?.Kill();
		}

		public void ApplyStyle(EarnedRewardData style)
		{
			Prepare(style.Reward, true);
			IncreaseAmount(style.Amount, 0.0f);
		}

		public void Prepare(RewardDefinition reward, bool visible)
		{
			_rewardIcon.sprite = reward.Sprite;
			SetVisible(visible);
		}

		public void SetVisible(bool visible)
		{
			_canvasGroup.alpha = visible ? 1.0f : 0.0f;
		}

		public void IncreaseAmount(int amount, float duration)
		{
			_amountIncreaseTween?.Kill();

			_cachedAmount = _amount + amount;

			if (duration <= 0.0f)
			{
				_amount = _cachedAmount;
				SetAmount(_amount);
				return;
			}

			_amountIncreaseTween = DOTween.To(() => _amount, value => _amount = value, _cachedAmount, duration)
				.OnUpdate(() => SetAmount(_amount))
				.SetEase(Ease.OutQuad)
				.OnKill(OnAmountIncreaseKilled);
		}

		public Tween PlayLanding(float duration)
		{
			transform.DOKill();
			transform.localScale = Vector3.one;

			return transform.DOPunchScale(Vector3.one * 0.15f, duration, 8, 0.5f)
				.SetEase(Ease.OutQuad);
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
