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
		private ZoneProgressBarController _zoneProgressBarController;

		[SerializeField]
		private CurrencyPanelController _currencyPanelController;

		private void OnValidate()
		{
			_gameSession = GetComponent<GameSession>();
		}

		private void Start()
		{
			_zoneIndicatorPanelController.Initialize();
			_zoneProgressBarController.Initialize(_gameSession.ZoneSchedule);

			_gameSession.ZoneEntered += OnZoneEntered;
			_gameSession.UpcomingSpecialZoneChanged += OnUpcomingSpecialZoneChanged;
			_gameSession.CurrencyChanged += OnCurrencyChanged;
		}

		private void OnDestroy()
		{
			_gameSession.ZoneEntered -= OnZoneEntered;
			_gameSession.UpcomingSpecialZoneChanged -= OnUpcomingSpecialZoneChanged;
			_gameSession.CurrencyChanged -= OnCurrencyChanged;
		}

		private void OnZoneEntered(int zoneIndex, ZoneDefinition zoneDefinition)
		{
			_zoneProgressBarController.SetCurrentZone(zoneIndex, zoneDefinition);
		}

		private void OnUpcomingSpecialZoneChanged(ZoneDefinition zoneDefinition, int zoneIndex, RewardDefinition reservedReward)
		{
			_zoneIndicatorPanelController.SetIndicator(zoneDefinition, zoneIndex, reservedReward);
		}

		private void OnCurrencyChanged(int currencyAmount)
		{
			_currencyPanelController.SetCurrencyAmount(currencyAmount);
		}
	}
}
