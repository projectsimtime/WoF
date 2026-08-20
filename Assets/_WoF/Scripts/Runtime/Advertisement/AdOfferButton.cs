using WoF.Core;
using WoF.UI;

namespace WoF.Advertisement
{
	/// <summary>
	/// Base for every "watch an ad" offer. Subclasses only declare which placement they are;
	/// the reward and the ad length come from data.
	/// </summary>
	public abstract class AdOfferButton : ButtonController
	{
		private IAdOfferService _adOfferService;

		protected abstract EAdPlacement Placement { get; }

		protected override void OnInject(IServiceResolver services)
		{
			_adOfferService = services.Resolve<IAdOfferService>();
		}

		protected override void OnButtonClicked()
		{
			_adOfferService?.Play(Placement);
		}
	}
}
