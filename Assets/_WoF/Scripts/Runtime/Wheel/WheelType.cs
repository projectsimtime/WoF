using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF
{
	public enum EWheelType
	{
		Bronze = 0,
		Silver,
		Gold
	}
	
	[Serializable]
	public struct WheelTypeContent
	{
		[SerializeField] 
		private List<RewardDefinition> _rewards;

		public List<RewardDefinition> Rewards => _rewards;

		[SerializeField] 
		public Dictionary<EItemRarity, List<RewardDefinition>> RewardByRarity;
	}
	
	[CreateAssetMenu(fileName = "wheel_", menuName = "WoF/Wheel/New Wheel Type")]
	public class WheelType : ScriptableObject
	{
		[SerializeField] 
		private Sprite _wheelSprite;
		[SerializeField] 
		private Sprite _indicatorSprite;
		
		[SerializeField] 
		private WheelTypeContent _wheelTypeContent;
		
		public Sprite WheelSprite => _wheelSprite;
		public Sprite IndicatorSprite => _indicatorSprite;
		
		public WheelTypeContent WheelTypeContent => _wheelTypeContent;

		private void OnEnable()
		{
			_wheelTypeContent.RewardByRarity = new Dictionary<EItemRarity, List<RewardDefinition>>();
			
			foreach (RewardDefinition reward in _wheelTypeContent.Rewards)
			{
				if (reward)
				{
					if (!_wheelTypeContent.RewardByRarity.TryAdd(reward.Rarity, new List<RewardDefinition> { reward }))
					{
						_wheelTypeContent.RewardByRarity[reward.Rarity].Add(reward);
					}
				}
			}
		}
	}
}