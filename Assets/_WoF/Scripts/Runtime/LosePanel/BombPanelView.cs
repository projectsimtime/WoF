using DG.Tweening;
using UnityEngine;

namespace WoF.LosePanel
{
	public class BombPanelView : MonoBehaviour
	{
		private static readonly Vector3 HiddenScale = Vector3.one * 0.85f;

		private Tween _scaleTween;

		private void OnDisable()
		{
			ResetView();
		}

		public void ResetView()
		{
			_scaleTween?.Kill();
			transform.localScale = Vector3.one;
		}

		public Tween PlayEnter(float duration)
		{
			transform.localScale = HiddenScale;
			return PlayScale(Vector3.one, duration, Ease.OutBack);
		}

		public Tween PlayExit(float duration)
		{
			return PlayScale(HiddenScale, duration, Ease.InBack);
		}

		private Tween PlayScale(Vector3 targetScale, float duration, Ease ease)
		{
			_scaleTween?.Kill();
			_scaleTween = transform.DOScale(targetScale, duration)
				.SetEase(ease)
				.OnKill(() => _scaleTween = null);

			return _scaleTween;
		}
	}
}
