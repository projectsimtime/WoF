using DG.Tweening;
using UnityEngine;

namespace WoF.Advertisement
{
	public class AdPanelController : PanelController
	{
		[SerializeField]
		private SkipButton _skipButton;

		[SerializeField]
		private CollectButton _collectButton;
		
		private AdRewardBase _pendingReward;

		private float _remainingTime;

		private Tween _countdownTween;
		
		protected void OnValidate()
		{
			_skipButton = GetComponentInChildren<SkipButton>(true);
			_collectButton = GetComponentInChildren<CollectButton>(true);
		}

		public void Play(AdRewardBase reward, float duration)
		{
			_pendingReward = reward;

			Show();
			StartCountdown(duration);
		}

		public void Collect()
		{
			_pendingReward?.GiveAdReward();
			Close();
		}

		public void Skip()
		{
			Close();
		}

		private void Close()
		{
			StopCountdown();
			_pendingReward = null;
			
			Hide();
		}

		private void StartCountdown(float duration)
		{
			_remainingTime = duration;

			_collectButton.SetButtonInteractable(false);
			
			_countdownTween = DOTween.To(() => _remainingTime, value => _remainingTime = value, 0.0f, duration)
				.OnUpdate(SetCountdownText)
				.SetUpdate(UpdateType.Fixed)
				.SetEase(Ease.Linear)
				.OnComplete(OnCountdownCompleted);
		}
		private void StopCountdown()
		{
			_countdownTween?.Kill();
		}
		
		private void OnCountdownCompleted()
		{
			_collectButton.SetButtonInteractable(true);
			_collectButton.SetButtonText("COLLECT AD REWARDS");
		}

		void SetCountdownText()
		{
			_collectButton.SetButtonText(_remainingTime.ToString("F1"));
		}
	}
}