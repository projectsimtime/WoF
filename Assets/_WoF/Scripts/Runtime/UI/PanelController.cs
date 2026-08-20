using DG.Tweening;
using UnityEngine;

namespace WoF.UI
{
	/// <summary>
	/// Base for full screen panels. Show and hide fade instead of popping, which is what the
	/// panel traffic between wheel, bomb, exit and hint used to look like.
	/// </summary>
	public abstract class PanelController : MonoBehaviour
	{
		private const float TransitionDuration = 0.16f;

		private CanvasGroup _canvasGroup;
		private Tween _transitionTween;

		protected virtual void Awake()
		{
			EnsureCanvasGroup();
		}

		protected virtual void OnDestroy()
		{
			KillTransition();
		}

		public virtual void Show()
		{
			EnsureCanvasGroup();
			KillTransition();

			gameObject.SetActive(true);

			_canvasGroup.alpha = 0.0f;
			_canvasGroup.blocksRaycasts = true;

			_transitionTween = _canvasGroup
				.DOFade(1.0f, TransitionDuration)
				.SetEase(Ease.OutQuad)
				.OnKill(OnShowKilled);
		}

		public virtual void Hide()
		{
			EnsureCanvasGroup();
			KillTransition();

			if (!gameObject.activeSelf)
			{
				return;
			}

			_canvasGroup.blocksRaycasts = false;

			_transitionTween = _canvasGroup
				.DOFade(0.0f, TransitionDuration)
				.SetEase(Ease.InQuad)
				.OnKill(OnHideKilled);
		}

		private void OnShowKilled()
		{
			_transitionTween = null;

			if (_canvasGroup != null)
			{
				_canvasGroup.alpha = 1.0f;
			}
		}

		private void OnHideKilled()
		{
			_transitionTween = null;

			if (_canvasGroup != null)
			{
				_canvasGroup.alpha = 1.0f;
			}

			gameObject.SetActive(false);
		}

		private void KillTransition()
		{
			if (_transitionTween == null)
			{
				return;
			}

			Tween transitionTween = _transitionTween;
			_transitionTween = null;

			transitionTween.Kill();
		}

		private void EnsureCanvasGroup()
		{
			if (_canvasGroup != null)
			{
				return;
			}

			_canvasGroup = GetComponent<CanvasGroup>();

			if (_canvasGroup == null)
			{
				_canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
		}
	}
}
