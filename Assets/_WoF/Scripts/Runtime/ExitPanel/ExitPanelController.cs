using UnityEngine;
using UnityEngine.Serialization;
using WoF.UI;

namespace WoF.ExitPanel
{
	public class ExitPanelController : PanelController
	{
		[SerializeField]
		private ExitPanelCollectRewardButton exitPanelCollectRewardButton;
		[SerializeField]
		private ExitPanelGoBackButton exitPanelGoBackButton;

		private void OnValidate()
		{
			exitPanelCollectRewardButton = GetComponentInChildren<ExitPanelCollectRewardButton>();
			exitPanelGoBackButton = GetComponentInChildren<ExitPanelGoBackButton>();
		}
	}
}
