using DG.Tweening;
using UnityEngine;
using WoF.UI;

namespace WoF.Advertisement
{
	public class AdPanelController : PanelController, IAdPlayer, IAdPlaybackControls
	{
		private const string CollectLabel = "COLLECT AD REWARDS";
		private const string CountdownFormat = "F1";

		[SerializeField]
		private SkipButton _skipButton;

		[SerializeField]
		private CollectButton _collectButton;

		private IAdReward _pendingReward;

		private float _remainingTime;

		private Tween _countdownTween;

		private void OnValidate()
		{
			_skipButton = GetComponentInChildren<SkipButton>(true);
			_collectButton = GetComponentInChildren<CollectButton>(true);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			StopCountdown();
		}

		public void Play(IAdReward reward, float duration)
		{
			_pendingReward = reward;

			Show();
			StartCountdown(duration);
		}

		public void Collect()
		{
			IAdReward pendingReward = _pendingReward;

			Close();

			pendingReward?.Grant();
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
			StopCountdown();

			_remainingTime = duration;

			_collectButton.SetInteractable(false);
			SetCountdownText();

			_countdownTween = DOTween.To(() => _remainingTime, value => _remainingTime = value, 0.0f, duration)
				.OnUpdate(SetCountdownText)
				.SetUpdate(UpdateType.Fixed)
				.SetEase(Ease.Linear)
				.OnComplete(OnCountdownCompleted);
		}

		private void StopCountdown()
		{
			if (_countdownTween == null)
			{
				return;
			}

			Tween countdownTween = _countdownTween;
			_countdownTween = null;

			countdownTween.Kill();
		}

		private void OnCountdownCompleted()
		{
			_countdownTween = null;

			_collectButton.SetInteractable(true);
			_collectButton.SetButtonText(CollectLabel);
		}

		private void SetCountdownText()
		{
			_collectButton.SetButtonText(_remainingTime.ToString(CountdownFormat));
		}
	}
}
