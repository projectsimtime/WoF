using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF.Wheel
{
	[Serializable]
	public struct RarityWeight
	{
		[SerializeField]
		private RarityDefinition _rarity;
		[Min(1)]
		[SerializeField]
		private int _weight;

		public RarityDefinition Rarity => _rarity;
		public int Weight => _weight;
	}
	
	[Serializable]
	public struct WheelTypeContent
	{
		[SerializeField] 
		private RewardDefinition[] _rewards;
		[SerializeField]
		private RarityWeight[] _rarityWeights;

		public RewardDefinition[] Rewards => _rewards;
		public RarityWeight[] RarityWeights => _rarityWeights;
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

		private Dictionary<RarityDefinition, List<RewardDefinition>> _rewardByRarity;
		
		public Sprite WheelSprite => _wheelSprite;
		public Sprite IndicatorSprite => _indicatorSprite;
		
		public WheelTypeContent WheelTypeContent => _wheelTypeContent;

		private void OnEnable()
		{
			_rewardByRarity = new Dictionary<RarityDefinition, List<RewardDefinition>>();
			
			foreach (RewardDefinition reward in _wheelTypeContent.Rewards)
			{
				if (reward)
				{
					if (!_rewardByRarity.TryAdd(reward.Rarity, new List<RewardDefinition> { reward }))
					{
						_rewardByRarity[reward.Rarity].Add(reward);
					}
				}
			}
		}

		public List<RewardDefinition> GetRewardsByRarity(RarityDefinition rarity)
		{
			return _rewardByRarity[rarity];
		}
	}
}
