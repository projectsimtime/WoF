using System.Collections.Generic;

namespace WoF.Advertisement
{
	/// <summary>
	/// Maps a placement to the reward it grants and to how long its ad runs.
	/// Both maps are supplied by the composition root, so this class never grows a branch.
	/// </summary>
	public class AdOfferService : IAdOfferService
	{
		private const float FallbackDuration = 3.0f;

		private readonly IAdPlayer _adPlayer;
		private readonly IReadOnlyDictionary<EAdPlacement, IAdReward> _rewardsByPlacement;
		private readonly IReadOnlyDictionary<EAdPlacement, float> _durationsByPlacement;

		public AdOfferService(
			IAdPlayer adPlayer,
			IReadOnlyDictionary<EAdPlacement, IAdReward> rewardsByPlacement,
			IReadOnlyDictionary<EAdPlacement, float> durationsByPlacement)
		{
			_adPlayer = adPlayer;
			_rewardsByPlacement = rewardsByPlacement;
			_durationsByPlacement = durationsByPlacement;
		}

		public void Play(EAdPlacement placement)
		{
			if (!_rewardsByPlacement.TryGetValue(placement, out IAdReward reward))
			{
				return;
			}

			if (!_durationsByPlacement.TryGetValue(placement, out float duration))
			{
				duration = FallbackDuration;
			}

			_adPlayer.Play(reward, duration);
		}
	}
}
