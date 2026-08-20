using WoF.Core;
using WoF.UI;

namespace WoF.Advertisement
{
	public class CollectButton : LabeledButtonController
	{
		private IAdPlaybackControls _adPlaybackControls;

		protected override void OnInject(IServiceResolver services)
		{
			_adPlaybackControls = services.Resolve<IAdPlaybackControls>();
		}

		protected override void OnButtonClicked()
		{
			_adPlaybackControls?.Collect();
		}
	}
}
