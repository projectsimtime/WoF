using UnityEngine;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelController : PanelController, IExitView
	{
		[SerializeField]
		private ExitPanelCollectRewardButton _collectRewardButton;
		[SerializeField]
		private ExitPanelGoBackButton _goBackButton;

		private void OnValidate()
		{
			_collectRewardButton = GetComponentInChildren<ExitPanelCollectRewardButton>(true);
			_goBackButton = GetComponentInChildren<ExitPanelGoBackButton>(true);
		}
	}
}
