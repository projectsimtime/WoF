using UnityEngine;

namespace WoF
{
	public class ReviveCostCalculator
	{
		private int _reviveCostScale;
		private int _reviveCost;

		private int _revivedTime;

		public ReviveCostCalculator(int reviveCost, int reviveCostScale)
		{
			_reviveCost = reviveCost;
			_reviveCostScale = reviveCostScale;
		}

		public int GetReviveCost()
		{
			return _reviveCost + (_revivedTime * _reviveCostScale);
		}

		public void OnRevived()
		{
			++_revivedTime;
		}

		public void Reset()
		{
			_revivedTime = 0;
		}
	}
}
