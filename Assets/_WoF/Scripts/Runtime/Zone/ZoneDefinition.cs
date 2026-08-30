using UnityEngine;
using WoF.Reward;
using WoF.Wheel;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneDefinition_", menuName = "WoF/Zone/New Zone Definition", order = 0)]
	public class ZoneDefinition : ScriptableObject
	{
		[Header("Presentation")]
		[SerializeField]
		private string _label;
		[SerializeField]
		private Color _themeColor;
		[SerializeField]
		private Sprite _icon;

		[Header("Gameplay")]
		[SerializeField]
		private WheelType _wheelType;
		[SerializeField]
		private bool _canExit;

		[Header("Rewards")]
		[SerializeField]
		private RewardDefinition[] _reservedRewards;

		public string Label => _label;
		public Color ThemeColor => _themeColor;
		public Sprite Icon => _icon;
		public WheelType WheelType => _wheelType;
		public bool CanExit => _canExit;
		public RewardDefinition[] ReservedRewards => _reservedRewards;
	}
}
