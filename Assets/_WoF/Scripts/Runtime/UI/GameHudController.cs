using UnityEngine;
using WoF.Currency;
using WoF.Reward;
using WoF.Zone;
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
		private CurrentZonePanelController _currentZonePanelController;

		[SerializeField]
		private CurrencyPanelController _currencyPanelController;

		private void OnValidate()
		{
			_gameSession = GetComponent<GameSession>();
		}

		private void Awake()
		{
			_zoneIndicatorPanelController.Initialize();
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
			_currentZonePanelController.SetZoneIndex(zoneIndex, zoneDefinition.ThemeColor);
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
