using System.Collections.Generic;
using WoF.ExitPanel;
using WoF.HintPanel;
using WoF.Reward;
using WoF.Zone;

namespace WoF.Session
{
	/// <summary>
	/// The next zone hint. It builds the upcoming zone early and hands that same wheel back
	/// to the run later, so what the hint promised is what the player actually gets.
	/// </summary>
	public class HintService : IHintCommands
	{
		private readonly ZoneWheelFactory _zoneWheelFactory;
		private readonly RewardService _rewardService;
		private readonly RunState _runState;
		private readonly IHintView _hintView;
		private readonly IExitView _exitView;
		private readonly int _endGameZoneIndex;

		public HintService(
			ZoneWheelFactory zoneWheelFactory,
			RewardService rewardService,
			RunState runState,
			IHintView hintView,
			IExitView exitView,
			int endGameZoneIndex)
		{
			_zoneWheelFactory = zoneWheelFactory;
			_rewardService = rewardService;
			_runState = runState;
			_hintView = hintView;
			_exitView = exitView;
			_endGameZoneIndex = endGameZoneIndex;
		}

		public void RequestHint()
		{
			int nextZoneIndex = _runState.CurrentZone + 1;

			if (nextZoneIndex > _endGameZoneIndex)
			{
				return;
			}

			ZoneWheel nextZoneWheel = _zoneWheelFactory.Peek(nextZoneIndex);
			RewardDefinition bestReward = GetBestReward(nextZoneWheel.Rewards);

			if (bestReward == null)
			{
				return;
			}

			_exitView.Hide();
			_hintView.Show();
			_hintView.ShowHint(bestReward, _rewardService.GetAmount(bestReward, nextZoneIndex));
		}

		public void CloseHint()
		{
			_hintView.Hide();
			_exitView.Show();
		}

		private static RewardDefinition GetBestReward(IReadOnlyList<RewardDefinition> rewards)
		{
			if (rewards == null)
			{
				return null;
			}

			RewardDefinition bestReward = null;

			foreach (RewardDefinition reward in rewards)
			{
				if (reward == null || reward.IsBomb)
				{
					continue;
				}

				if (bestReward == null || reward.Rarity > bestReward.Rarity)
				{
					bestReward = reward;
				}
			}

			return bestReward;
		}
	}
}
