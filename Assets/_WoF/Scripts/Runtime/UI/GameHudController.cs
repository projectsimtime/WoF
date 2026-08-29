using UnityEngine;
using WoF.Currency;
using WoF.Reward;
using WoF.Zone;
using WoF.ZoneProgress;
using WoF.ZoneView;

namespace WoF.UI
{
	public class GameHudController : MonoBehaviour
	{
		[SerializeField]
		private GameSession _gameSession;

		[SerializeField]
		private ZoneIndicatorPanelController _zoneIndicatorPanelController;

		[SerializeField]
		private ZoneProgressBarView _zoneProgressBarView;

		[SerializeField]
		private CurrencyPanelController _currencyPanelController;

		private void OnValidate()
		{
			_gameSession = GetComponent<GameSession>();
		}

		private void Start()
		{
			_zoneIndicatorPanelController.Initialize();
			_zoneProgressBarView.Initialize(_gameSession.ZoneTypeCalculator, _gameSession.EndGameZoneIndex);
			_gameSession.ZoneEntered += OnZoneEntered;
			_gameSession.UpcomingZoneChanged += OnUpcomingZoneChanged;
			_gameSession.CurrencyChanged += OnCurrencyChanged;
		}

		private void OnDestroy()
		{
			_gameSession.ZoneEntered -= OnZoneEntered;
			_gameSession.UpcomingZoneChanged -= OnUpcomingZoneChanged;
			_gameSession.CurrencyChanged -= OnCurrencyChanged;
		}

		private void OnZoneEntered(int zoneIndex, ZoneTypeData _)
		{
			_zoneProgressBarView.SetCurrentZone(zoneIndex);
		}

		private void OnUpcomingZoneChanged(ZoneTypeData zoneTypeData, int zoneIndex, RewardDefinition reservedReward)
		{
			_zoneIndicatorPanelController.SetIndicator(zoneTypeData, zoneIndex, reservedReward);
		}

		private void OnCurrencyChanged(int currencyAmount)
		{
			_currencyPanelController.SetCurrencyAmount(currencyAmount);
		}
	}
}
