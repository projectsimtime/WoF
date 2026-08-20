using System;
using WoF.Currency;
using WoF.Session;
using WoF.Wheel;

namespace WoF.LosePanel
{
	/// <summary>
	/// Owns everything that happens between hitting the bomb and being back on the wheel:
	/// the price of a second chance, who can pay it, and what a revive does to the wheel.
	/// </summary>
	public class ReviveService : IReviveCommands
	{
		private readonly EconomyService _economyService;
		private readonly ReviveCostCalculator _reviveCostCalculator;
		private readonly ILoseView _loseView;
		private readonly IWheelView _wheelView;
		private readonly RunState _runState;

		public ReviveService(
			EconomyService economyService,
			ReviveCostCalculator reviveCostCalculator,
			ILoseView loseView,
			IWheelView wheelView,
			RunState runState)
		{
			_economyService = economyService;
			_reviveCostCalculator = reviveCostCalculator;
			_loseView = loseView;
			_wheelView = wheelView;
			_runState = runState;
		}

		public event Action Revived;
		public event Action GaveUp;

		public void ShowLoseScreen()
		{
			_loseView.Show();
		}

		public void ReviveWithCurrency()
		{
			int reviveCost = _reviveCostCalculator.GetReviveCost();

			if (!_economyService.CanAfford(reviveCost))
			{
				_loseView.SetReviveEnabled(false);

				return;
			}

			_economyService.Spend(reviveCost);
			_reviveCostCalculator.OnRevived();

			RefreshReviveOffer();
			Resume();
		}

		public void ReviveWithAd()
		{
			_loseView.SetAdOfferVisible(false);

			Resume();
		}

		public void GiveUp()
		{
			_loseView.Hide();

			GaveUp?.Invoke();
		}

		public void ResetForNewRun()
		{
			_reviveCostCalculator.Reset();

			_loseView.Hide();
			_loseView.SetAdOfferVisible(true);

			RefreshReviveOffer();
		}

		private void RefreshReviveOffer()
		{
			int nextReviveCost = _reviveCostCalculator.GetReviveCost();

			_loseView.ShowReviveCost(nextReviveCost);
			_loseView.SetReviveEnabled(_economyService.CanAfford(nextReviveCost));
		}

		private void Resume()
		{
			_loseView.Hide();

			_runState.IsResumingAfterBomb = true;

			_wheelView.SetSlotActive(_runState.ReservedSlotIndex, false);

			Revived?.Invoke();
		}
	}
}
