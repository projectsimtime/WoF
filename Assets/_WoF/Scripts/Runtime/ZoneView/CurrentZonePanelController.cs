using UnityEngine;
using UnityEngine.UI;

namespace WoF.ZoneView
{
	public class CurrentZonePanelController : MonoBehaviour, IZoneHudView
	{
		[SerializeField]
		private CurrentZonePanelValue _currentZone;
		[SerializeField]
		private Image _frame;

		private void OnValidate()
		{
			_currentZone = GetComponentInChildren<CurrentZonePanelValue>(true);
		}

		public void ShowZone(int zoneIndex, Color themeColor)
		{
			_currentZone.SetZoneIndex(zoneIndex, themeColor);

			if (_frame != null)
			{
				_frame.color = themeColor;
			}
		}
	}
}
