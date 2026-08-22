using System;
using System.Collections.Generic;
using UnityEngine;
using WoF.Reward;

namespace WoF.Wheel
{
	[Serializable]
	public struct RarityChance
	{
		[SerializeField]
		private EItemRarity _rarity;
		[Range(0, 100)]
		[SerializeField]
		private int _chance;

		public EItemRarity Rarity => _rarity;
		public int Chance => _chance;
	}
	
	[Serializable]
	public struct WheelTypeContent
	{
		[SerializeField] 
		private List<RewardDefinition> _rewards;
		[SerializeField]
		private RarityChance[] _rarityChances;

		public List<RewardDefinition> Rewards => _rewards;
		public RarityChance[] RarityChances => _rarityChances;

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
