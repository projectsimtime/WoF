using UnityEngine;

namespace WoF
{
	public class CurrentZonePanelController : MonoBehaviour
	{
		[SerializeField] private CurrentZonePanelValue _currentZone;

		private void OnValidate()
		{
			_currentZone = GetComponentInChildren<CurrentZonePanelValue>(true);
		}

		public void SetZoneIndex(int zoneIndex, Color color)
		{
			_currentZone.SetZoneIndex(zoneIndex, color);
		}
	}
}
