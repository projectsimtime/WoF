using UnityEngine;

namespace WoF
{
	public class WheelParent : MonoBehaviour
	{
		
		[SerializeField]
		private WheelSpin _wheelSpin;
		[SerializeField]
		private WheelIndicator _wheelIndicator;

		[SerializeField] 
		private WheelType[] WheelTypes;
		
		private void OnValidate()
		{
			_wheelSpin = GetComponentInChildren<WheelSpin>();
			_wheelIndicator = GetComponentInChildren<WheelIndicator>();
		}
		
		public void ApplyWheelType(WheelType wheelType)
		{
			_wheelSpin.ApplyStyle(wheelType);
			_wheelIndicator.ApplyStyle(wheelType);
		}
		
	}
}