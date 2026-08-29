using UnityEngine;
using WoF.Reward;
using WoF.Wheel;

namespace WoF.Zone
{
	[CreateAssetMenu(fileName = "zoneType_", menuName = "WoF/Zone/New Zone Type", order = 0)]
	public class ZoneTypeData : ScriptableObject
	{
		[Header("Theme")]
		[SerializeField]
		private string _label;
		[SerializeField]
		private Color _themeColor;
		[SerializeField]
		private Sprite _icon;

		[Header("Rules")]
		[SerializeField]
		private WheelType _wheelType;
		[SerializeField]
		private int _frequency = 1;
		[SerializeField]
		private int _priority;
		[SerializeField]
		private int[] _overrideIndices;
		[SerializeField]
		private bool _canExit;

		[Header("Rewards")]
		[SerializeField]
		private RewardDefinition[] _reservedRewards;

		public string Label => _label;
		public Color ThemeColor => _themeColor;
		public Sprite Icon => _icon;
		public WheelType WheelType => _wheelType;
		public int Frequency => _frequency;
		public int Priority => _priority;
		public int[] OverrideIndices => _overrideIndices;
		public bool CanExit => _canExit;
		public RewardDefinition[] ReservedRewards => _reservedRewards;
	}
}
