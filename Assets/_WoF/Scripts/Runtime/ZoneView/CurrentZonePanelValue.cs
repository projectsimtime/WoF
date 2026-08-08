using DG.Tweening;
using TMPro;
using UnityEngine;

namespace WoF
{
	public class CurrentZonePanelValue : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;

		private Tween _zoneChangeTween;

		private void OnValidate()
		{
			_text = GetComponent<TextMeshProUGUI>();
		}

		private void OnDisable()
		{
			_zoneChangeTween?.Kill();
		}

		public void SetZoneIndex(int zoneIndex, Color color)
		{
			_text.text = zoneIndex.ToString();
			_text.color = color;

			_zoneChangeTween?.Kill();
			_zoneChangeTween = transform.DOPunchScale(Vector3.one * 0.3f, 0.4f)
				.SetEase(Ease.OutQuad)
				.OnKill(OnZoneChangeKilled);
		}

		private void OnZoneChangeKilled()
		{
			transform.localScale = Vector3.one;
		}
	}
}
