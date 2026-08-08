using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using WoF.Reward;
using WoF.Zone;
using Random = UnityEngine.Random;

namespace WoF
{
	public class GameSession : MonoBehaviour
	{
		private struct ZoneWheel
		{
			public WheelType WheelType;
			public List<RewardDefinition> Rewards;
			public int ReservedSlotIndex;
		}

		[SerializeField]
		private ZoneRules _zoneRule;
		
		private Dictionary<int, int> _zoneOverrides;

		private int _currentZone = 1;

		private int _reservedSlotIndex;
		
		[SerializeField] 
		private WheelType[] _wheelTypes;

		[SerializeField] 
		private WheelParent _wheelParent;

		[SerializeField]
		private RewardDefinition _bombReward;

		[SerializeField]
		private RewardDefinition[] _specialItems;

		[SerializeField]
		private GameSettings _gameSettings;

		[SerializeField]
		private RewardAmountMap _rewardAmountMap;

		private RewardDefinition _earnedReward;

		private List<RewardDefinition> _rewardDefinitions = new();

		private bool _isContinueAfterBomb;

		[SerializeField]
		private LosePanelController _losePanelController;

		[SerializeField]
		private ExitPanelController _exitPanelController;

		private EarnedRewardContainer _rewardContainer;

		[SerializeField] 
		private RewardPanelController _rewardPanelController;

		[SerializeField]
		private ZoneIndicatorPanelController _zoneIndicatorPanelController;
		
		[SerializeField]
		private CurrencyPanelController _currencyPanelController;
		[SerializeField]
		private EndGamePanelController _endGamePanelController;
		[SerializeField]
		private RewardSummaryPanelController _rewardSummaryPanelController;

		private ZoneCalculator _zoneCalculator;
		private ReviveCostCalculator _reviveCostCalculator;
		private SpinAngleCalculator _spinAngleCalculator;
		private RarityCalculator _rarityCalculator;

		private RewardDefinition[] _superZoneRewards;

		private bool _isWheelSpinning;

		private CurrencyBag _currencyBag;
		
		

		bool CanExitNow()
		{
			return _zoneCalculator.IsSpecialZone(_currentZone) && 
			       !_isWheelSpinning;
		}


		[SerializeField] 
		private ExitButton _exitButton;

		private void Awake()
		{
			_rewardContainer = new EarnedRewardContainer();
			_zoneCalculator = new ZoneCalculator(_gameSettings.SafeZoneFrequency, _gameSettings.SuperZoneFrequency, _gameSettings.EndGameZoneIndex);
			_reviveCostCalculator = new ReviveCostCalculator(_gameSettings.InitReviveCost, _gameSettings.ReviveCostScale);
			_spinAngleCalculator = new SpinAngleCalculator(_gameSettings.SlotCount, _gameSettings.SpinTargetAngle, _gameSettings.SpinSlotOffsetAngle, _gameSettings.SpinEdgeBias, _gameSettings.SpinNearMissChance);
			_rarityCalculator = new RarityCalculator(_gameSettings.SlotCount, _gameSettings.LegendaryChance, _gameSettings.EpicChance, _gameSettings.RareChance);

			_currencyBag = new CurrencyBag(_gameSettings.StartCurrencyAmount);
			
			_zoneOverrides = new Dictionary<int, int>();

			for (int i = 0; i < _zoneRule.ZoneOverrides.Length; ++i)
			{
				_zoneOverrides.Add(_zoneRule.ZoneOverrides[i].ZoneNumber, i);
			}
		}

		private void CalculateSuperZoneRewards()
		{
			int superZoneCount = _gameSettings.SuperZoneCount;

			_superZoneRewards = new RewardDefinition[superZoneCount];
			
			int randomIndex = Random.Range(0, _specialItems.Length);

			for (int i = 0; i < superZoneCount; ++i)
			{
				_superZoneRewards[i] = _specialItems[randomIndex];

				randomIndex = (randomIndex + 1) % _specialItems.Length;
			}
		}

		private RewardDefinition GetSuperZoneReward(int superZoneIndex)
		{
			int rewardIndex = (superZoneIndex / _gameSettings.SuperZoneFrequency) - 1;

			return _superZoneRewards[rewardIndex];
		}

		private void OnValidate()
		{
			_rewardPanelController = FindObjectOfType<RewardPanelController>(true);
			_zoneIndicatorPanelController = FindObjectOfType<ZoneIndicatorPanelController>(true);
			_exitPanelController = FindObjectOfType<ExitPanelController>(true);
			_losePanelController = FindObjectOfType<LosePanelController>(true);
			_currencyPanelController = FindObjectOfType<CurrencyPanelController>(true);
			_endGamePanelController = FindObjectOfType<EndGamePanelController>(true);
			_rewardSummaryPanelController = FindObjectOfType<RewardSummaryPanelController>(true);
		}

		private void BuildCurrentZone()
		{
			_exitButton.SetButtonInteractable(CanExitNow());
			_wheelParent.RestoreWheelRotation();
			if (_isContinueAfterBomb)
			{
				_wheelParent.SetActiveWheelSlot(_reservedSlotIndex, true);
			}

			ZoneWheel zoneWheel = BuildZoneWheel(_currentZone);

			_reservedSlotIndex = zoneWheel.ReservedSlotIndex;
			_rewardDefinitions = zoneWheel.Rewards;

			ApplyZoneWheel(zoneWheel, _currentZone);

			_isContinueAfterBomb = false;
		}

		public Tween OnSpinClicked()
		{
			_isWheelSpinning = true;
			_exitButton.SetButtonInteractable(CanExitNow());

			float angle = _spinAngleCalculator.CalculateTargetAngle(_reservedSlotIndex, _isContinueAfterBomb, out var earnedRewardIndex);

			float duration = Random.Range(_gameSettings.SpinMinDuration, _gameSettings.SpinMaxDuration);

			var tween = _wheelParent.PlaySpin(angle, duration);
			
			tween.onComplete += OnSpinComplete;

			_earnedReward = _rewardDefinitions[earnedRewardIndex];

			return tween;
		}

		public bool IsGameFinished()
		{
			return _currentZone > _gameSettings.EndGameZoneIndex;
		}

		public void DisplayEarnedRewards()
		{
			_rewardSummaryPanelController.Show();
			_rewardSummaryPanelController.DisplayEarnedRewards(_rewardContainer);
		}

		private void OnGameFinished()
		{
			_endGamePanelController.Show();
		}
		
		private void OnSpinComplete()
		{
			_isWheelSpinning = false;
			_exitButton.SetButtonInteractable(CanExitNow());

			if (IsBomb(_earnedReward))
			{
				OnBombExploded();
			}
			else
			{
				int earnedAmount = GetRewardAmount(_earnedReward, _currentZone);

				_rewardContainer.AddItem(_earnedReward, earnedAmount);
				_rewardPanelController.AddItem(_earnedReward, earnedAmount);

				GoToNextZone();
			}
		}

		public bool IsBomb(RewardDefinition reward)
		{
			return reward == _bombReward;
		}

		public int GetRewardAmount(RewardDefinition reward, int zoneIndex)
		{
			return _rewardAmountMap.GetAmountByKind(reward, zoneIndex);
		}

		public void OnBombExploded()
		{
			_losePanelController.Show();
		}

		private void Revive()
		{
			_losePanelController.Hide();
			_isContinueAfterBomb = true;

			_wheelParent.SetActiveWheelSlot(_reservedSlotIndex, false);
		}

		public void OnReviveWithCurrency()
		{
			Revive();

			_currencyBag.AddCurrency(-_reviveCostCalculator.GetReviveCost());
			_reviveCostCalculator.OnRevived();

			int nextReviveCost = _reviveCostCalculator.GetReviveCost();
			_losePanelController.UpdateReviveCost(nextReviveCost);

			if (!_currencyBag.HasEnoughCurrency(nextReviveCost))
			{
				_losePanelController.OnNotEnoughCurrencyToRevive();
			}

			_currencyPanelController.SetCurrencyAmount(_currencyBag.GetRemainingCurrencyAmount());
		}

		public void OnReviveWithAd()
		{
			_losePanelController.HideAdButton();

			Revive();
		}

		public void OnHintClicked()
		{
			Debug.Log("OnHintClicked");
		}
		
		public void OnGiveUpButtonClicked()
		{
			_losePanelController.Hide();
			StartNewRun();
		}

		public void OnExitClicked()
		{
			_exitPanelController.Show();
		}

		public void OnGoBackClicked()
		{
			_exitPanelController.Hide();
		}

		public void OnCollectRewardClicked()
		{
			_exitPanelController.Hide();
			DisplayEarnedRewards();
		}

		private void EnterZone()
		{
			BuildCurrentZone();
			RefreshZoneIndicators();
		}

		public void StartNewRun()
		{
			ClearRunState();
			EnterZone();
		}

		private void ClearRunState()
		{
			_currentZone = 1;
			_earnedReward = null;
			_exitButton.SetButtonInteractable(CanExitNow());

			_rewardDefinitions.Clear();
			_rewardPanelController.Clear();
			_rewardContainer.Clear();
			_currencyBag.Reset(_gameSettings.StartCurrencyAmount);

			_reviveCostCalculator.Reset();
			_losePanelController.UpdateReviveCost(_reviveCostCalculator.GetReviveCost());
			_currencyPanelController.SetCurrencyAmount(_currencyBag.GetRemainingCurrencyAmount());
			CalculateSuperZoneRewards();

			_rewardSummaryPanelController.Hide();
			_endGamePanelController.Hide();
		}

		private void GoToNextZone()
		{
			++_currentZone;

			if (IsGameFinished())
			{
				OnGameFinished();
				return;
			}

			EnterZone();
		}

		private void Start()
		{
			DOTween.Init();

			StartNewRun();
		}

		private void RefreshZoneIndicators()
		{
			int nextSuperZoneIndex = _zoneCalculator.GetNextSuperZoneIndex(_currentZone);

			_zoneIndicatorPanelController.OnNewZone(
				_zoneCalculator.GetNextSafeZoneIndex(_currentZone),
				nextSuperZoneIndex,
				GetSuperZoneReward(nextSuperZoneIndex).Sprite);
		}

		private EWheelType GetWheelType(int zoneIndex)
		{
			if (_zoneCalculator.IsSuperZone(zoneIndex))
			{
				return EWheelType.Gold;
			}

			if (_zoneCalculator.IsSafeZone(zoneIndex))
			{
				return EWheelType.Silver;
			}

			return EWheelType.Bronze;
		}

		private ZoneWheel BuildZoneWheel(int zoneIndex)
		{
			ZoneWheel zoneWheel;

			EWheelType wheelType = GetWheelType(zoneIndex);

			zoneWheel.WheelType = _wheelTypes[(int)wheelType];

			if (_zoneOverrides.TryGetValue(zoneIndex, out int index))
			{
				zoneWheel.Rewards = _zoneRule.ZoneOverrides[index].Rewards.ToList();
				zoneWheel.ReservedSlotIndex = zoneWheel.Rewards.IndexOf(_bombReward);

				if (zoneWheel.ReservedSlotIndex < 0)
				{
					zoneWheel.ReservedSlotIndex = Int32.MaxValue;
				}
			}
			else
			{
				bool hasBomb = wheelType == EWheelType.Bronze;
				bool hasSpecialItem = wheelType == EWheelType.Gold;

				zoneWheel.ReservedSlotIndex = hasBomb || hasSpecialItem ? Random.Range(0, _gameSettings.SlotCount) : Int32.MaxValue;
				zoneWheel.Rewards = GetZoneContents(zoneWheel.WheelType);

				if (zoneWheel.ReservedSlotIndex < zoneWheel.Rewards.Count)
				{
					zoneWheel.Rewards[zoneWheel.ReservedSlotIndex] = hasBomb ? _bombReward : GetSuperZoneReward(zoneIndex);
				}
			}

			return zoneWheel;
		}

		private void ApplyZoneWheel(ZoneWheel zoneWheel, int zoneIndex)
		{
			_wheelParent.ApplyWheelType(zoneWheel.WheelType);

			for (int i = 0; i < zoneWheel.Rewards.Count; ++i)
			{
				_wheelParent.ApplyWheelSlot(i, zoneWheel.Rewards[i], GetRewardAmount(zoneWheel.Rewards[i], zoneIndex));
			}
		}

		private List<RewardDefinition> GetZoneContents(WheelType wheelType)
		{
			var rewardsByRarity = wheelType.WheelTypeContent.RewardByRarity;

			var itemRarities = _rarityCalculator.GetItemRarityCount(wheelType);

			List<RewardDefinition> rewards = new List<RewardDefinition>(_gameSettings.SlotCount);

			foreach (var item in itemRarities)
			{
				EItemRarity rarity = item.Key;
				int count = item.Value;

				for (int i = 0; i < count; ++i)
				{
					List<RewardDefinition> currentRarityItems = rewardsByRarity[rarity];
					int rarityItemCount = currentRarityItems.Count;

					int randomIndex = Random.Range(0, rarityItemCount);

					rewards.Add(currentRarityItems[randomIndex]);
				}
			}

			return rewards;
		}
	}
}