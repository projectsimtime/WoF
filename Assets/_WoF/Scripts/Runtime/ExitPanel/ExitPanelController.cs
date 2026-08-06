using UnityEngine;

namespace WoF
{
	public class ExitPanelController : PanelController
	{
		[SerializeField]
		private CollectRewardButton _collectRewardButton;
		[SerializeField]
		private GoBackButton _goBackButton;

		private void OnValidate()
		{
			_collectRewardButton = GetComponentInChildren<CollectRewardButton>();
			_goBackButton = GetComponentInChildren<GoBackButton>();
		}
	}
}
