using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoF.Reward
{
	[CreateAssetMenu(fileName = "rewardAmountMap_", menuName = "WoF/Reward/New Reward Amount Map")]
	public class RewardAmountMap : ScriptableObject
	{
		[Serializable]
		public struct ItemAmountData
		{
			[SerializeField]
			private ItemKindDefinition _kind;
			[SerializeField]
			private int _baseAmount;
			[SerializeField]
			private int _maxAmount;
			[SerializeField] 
			private int _amountScale;

			public ItemKindDefinition Kind => _kind;
			public int BaseAmount => _baseAmount;
			public int MaxAmount => _maxAmount;
			public int AmountScale => _amountScale;
		}

		[SerializeField]
		private ItemAmountData[] _amountByKind;

		private Dictionary<ItemKindDefinition, ItemAmountData> _itemAmounts;
		
		private void OnEnable()
		{
			BuildMap();
		}

		private void BuildMap()
		{
			_itemAmounts = new Dictionary<ItemKindDefinition, ItemAmountData>();

			foreach (ItemAmountData itemAmountData in _amountByKind)
			{
				if (!_itemAmounts.TryAdd(itemAmountData.Kind, itemAmountData))
				{
					_itemAmounts[itemAmountData.Kind] = itemAmountData;
				}
			}
		}
		
		public int GetAmountByKind(RewardDefinition reward, int zone)
		{
			if (!reward.Kind || !_itemAmounts.TryGetValue(reward.Kind, out ItemAmountData itemAmountData))
			{
				return 1;
			}

			return Mathf.Min(itemAmountData.BaseAmount + Mathf.Max(itemAmountData.AmountScale, 1) * (zone - 1), itemAmountData.MaxAmount);
		}
	}
}
