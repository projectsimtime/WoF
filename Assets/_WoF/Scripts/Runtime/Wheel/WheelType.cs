using UnityEngine;

namespace WoF
{
	[CreateAssetMenu(fileName = "Wheel_", menuName = "New Wheel Type")]
	public class WheelType : ScriptableObject
	{
		[SerializeField] 
		private Sprite _wheelSprite;
		[SerializeField] 
		private Sprite _indicatorSprite;

		public Sprite WheelSprite => _wheelSprite;
		public Sprite IndicatorSprite => _indicatorSprite;
	}
}