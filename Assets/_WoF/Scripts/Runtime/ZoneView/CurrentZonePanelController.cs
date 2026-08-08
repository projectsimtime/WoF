using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
	public class CurrentZonePanelController : MonoBehaviour
	{
		[SerializeField] private CurrentZonePanelValue _currentZone;
		[SerializeField] private Image _frame;

		private void OnValidate()
		{
			_currentZone = GetComponentInChildren<CurrentZonePanelValue>(true);
		}

		public void SetZoneIndex(int zoneIndex, Color color)
		{
			_currentZone.SetZoneIndex(zoneIndex, color);

			_frame.color = color;
		}
	}
}
