using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoF.Reward
{
	[CreateAssetMenu(fileName = "rewardAmountMap_", menuName = "WoF/Reward/New Reward Amount Map")]
	public class RewardAmountMap : ScriptableObject, IRewardAmountProvider
	{
		private const int DefaultAmount = 1;

		[Serializable]
		public struct ItemAmountData
		{
			[SerializeField]
			private EItemKind _kind;
			[SerializeField]
			private int _baseAmount;
			[SerializeField]
			private int _maxAmount;
			[SerializeField]
			private int _amountScale;

			public EItemKind Kind => _kind;
			public int BaseAmount => _baseAmount;
			public int MaxAmount => _maxAmount;
			public int AmountScale => _amountScale;
		}

		[SerializeField]
		private ItemAmountData[] _amountByKind;

		private Dictionary<EItemKind, ItemAmountData> _itemAmounts;

		private void OnEnable()
		{
			BuildMap();
		}

		public int GetAmount(RewardDefinition reward, int zoneIndex)
		{
			if (reward == null)
			{
				return DefaultAmount;
			}

			if (_itemAmounts == null)
			{
				BuildMap();
			}

			if (!_itemAmounts.TryGetValue(reward.Kind, out ItemAmountData itemAmountData))
			{
				return DefaultAmount;
			}

			int scaledAmount = itemAmountData.BaseAmount + (Mathf.Max(itemAmountData.AmountScale, 1) * (zoneIndex - 1));

			return Mathf.Min(scaledAmount, itemAmountData.MaxAmount);
		}

		private void BuildMap()
		{
			_itemAmounts = new Dictionary<EItemKind, ItemAmountData>();

			if (_amountByKind == null)
			{
				return;
			}

			foreach (ItemAmountData itemAmountData in _amountByKind)
			{
				_itemAmounts[itemAmountData.Kind] = itemAmountData;
			}
		}
	}
}
