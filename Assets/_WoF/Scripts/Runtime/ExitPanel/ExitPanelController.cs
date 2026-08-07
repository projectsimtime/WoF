using UnityEngine;
using UnityEngine.Serialization;

namespace WoF
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
