using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WoF.Advertisement;
using WoF.Core;
using WoF.Currency;
using WoF.EndGame;
using WoF.ExitPanel;
using WoF.HintPanel;
using WoF.LosePanel;
using WoF.Reward;
using WoF.Session;
using WoF.Wheel;
using WoF.Zone;
using WoF.ZoneView;

namespace WoF
{
	/// <summary>
	/// The composition root, and the only place in the game that is allowed to know about
	/// concrete types. It collects the views the scene publishes, builds the services on top
	/// of them and hands each scene component the one contract it asked for.
	/// Nothing here decides anything about the game itself.
	/// </summary>
	public class GameBootstrap : MonoBehaviour
	{
		[Header("Configuration")]
		[SerializeField]
		private GameSettings _gameSettings;
		[SerializeField]
		private ZoneCatalog _zoneCatalog;
		[SerializeField]
		private ZoneRules _zoneRule;
		[SerializeField]
		private RewardAmountMap _rewardAmountMap;
		[SerializeField]
		private RewardDefinition _bombReward;

		private readonly ServiceContainer _container = new ServiceContainer();

		private GameFlow _gameFlow;

		private void Awake()
		{
			DOTween.Init();

			RegisterSceneViews();
			BuildServices();
			InjectSceneComponents();
		}

		private void Start()
		{
			_gameFlow.StartNewRun();
		}

		private void OnDestroy()
		{
			_container.Clear();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Development.DebugServices.Clear();
#endif
		}

		/// <summary>
		/// One scene walk, at start-up, instead of a FindObjectOfType per component.
		/// </summary>
		private void RegisterSceneViews()
		{
			foreach (ISceneService sceneService in SceneComponentScanner.FindAll<ISceneService>(gameObject.scene))
			{
				_container.RegisterSceneContracts(sceneService);
			}
		}

		private void BuildServices()
		{
			RunState runState = new RunState();

			ZoneCalculator zoneCalculator = new ZoneCalculator(_zoneCatalog, _gameSettings.EndGameZoneIndex);

			RarityCalculator rarityCalculator = new RarityCalculator(
				_gameSettings.SlotCount,
				_gameSettings.RarityChances,
				_gameSettings.BaseRarity);

			ZoneTypeData specialRewardZoneType = _zoneCatalog.GetSpecialRewardZoneType();

			MilestoneRewardPlanner milestoneRewards = new MilestoneRewardPlanner(
				specialRewardZoneType == null ? null : specialRewardZoneType.SpecialRewardPool);

			ZoneWheelFactory zoneWheelFactory = new ZoneWheelFactory(
				zoneCalculator,
				_zoneRule,
				rarityCalculator,
				milestoneRewards,
				_bombReward,
				_gameSettings.SlotCount);

			RewardService rewardService = new RewardService(
				new EarnedRewardContainer(),
				_rewardAmountMap,
				_container.Resolve<IRewardTrackView>(),
				_container.Resolve<IRewardSummaryView>());

			EconomyService economyService = new EconomyService(
				new CurrencyContainer(_gameSettings.StartCurrencyAmount),
				_container.Resolve<ICurrencyView>(),
				_gameSettings.StartCurrencyAmount);

			IWheelView wheelView = _container.Resolve<IWheelView>();

			ZoneRunner zoneRunner = new ZoneRunner(
				runState,
				zoneCalculator,
				_zoneCatalog,
				zoneWheelFactory,
				milestoneRewards,
				rewardService,
				wheelView,
				_container.Resolve<IZoneHudView>(),
				_container.Resolve<IZoneIndicatorView>());

			SpinService spinService = new SpinService(
				new SpinAngleCalculator(
					_gameSettings.SlotCount,
					_gameSettings.SpinTargetAngle,
					_gameSettings.SpinSlotOffsetAngle,
					_gameSettings.SpinEdgeBias,
					_gameSettings.SpinNearMissChance),
				wheelView,
				runState,
				_gameSettings.SpinMinDuration,
				_gameSettings.SpinMaxDuration,
				_gameSettings.SpinSettleDuration);

			ReviveService reviveService = new ReviveService(
				economyService,
				new ReviveCostCalculator(_gameSettings.InitReviveCost, _gameSettings.ReviveCostScale),
				_container.Resolve<ILoseView>(),
				wheelView,
				runState);

			ExitService exitService = new ExitService(
				zoneCalculator,
				runState,
				_container.Resolve<IExitView>());

			HintService hintService = new HintService(
				zoneWheelFactory,
				rewardService,
				runState,
				_container.Resolve<IHintView>(),
				_container.Resolve<IExitView>(),
				_gameSettings.EndGameZoneIndex);

			_gameFlow = new GameFlow(
				runState,
				zoneRunner,
				spinService,
				rewardService,
				reviveService,
				exitService,
				economyService,
				zoneWheelFactory,
				_container.Resolve<IEndGameView>(),
				_container.Resolve<IHintView>(),
				_gameSettings);

			_container.Register<ISpinCommands>(_gameFlow);
			_container.Register<IRunCommands>(_gameFlow);
			_container.Register<IDebugRunCommands>(_gameFlow);
			_container.Register<IExitCommands>(exitService);
			_container.Register<IReviveCommands>(reviveService);
			_container.Register<IHintCommands>(hintService);
			_container.Register<IAdOfferService>(BuildAdOfferService(reviveService, hintService));

#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Development.DebugServices.Publish(_container);
#endif
		}

		private IAdOfferService BuildAdOfferService(IReviveCommands reviveCommands, IHintCommands hintCommands)
		{
			Dictionary<EAdPlacement, IAdReward> rewardsByPlacement = new Dictionary<EAdPlacement, IAdReward>
			{
				{ EAdPlacement.Revive, new ReviveAdReward(reviveCommands) },
				{ EAdPlacement.ZoneHint, new ZoneHintAdReward(hintCommands) }
			};

			return new AdOfferService(
				_container.Resolve<IAdPlayer>(),
				rewardsByPlacement,
				_gameSettings.GetAdDurationsByPlacement());
		}

		private void InjectSceneComponents()
		{
			foreach (IInjectable injectable in SceneComponentScanner.FindAll<IInjectable>(gameObject.scene))
			{
				injectable.Inject(_container);
			}
		}
	}
}
