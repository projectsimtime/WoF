using UnityEngine;

namespace WoF
{
	[CreateAssetMenu(fileName = "zoneStaticView_", menuName = "WoF/Zone View/New Zone Static View", order = 0)]
	public class ZoneViewStaticData : ScriptableObject
	{
		[SerializeField] 
		private string _label;
		[SerializeField]
		private Color _themeColor;
		[SerializeField]
		private Sprite _icon;

		public string Label => _label;
		public Color ThemeColor => _themeColor;
		public Sprite Icon => _icon;
	}
}