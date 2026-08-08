using UnityEngine;
using WoF.UI;

namespace WoF.Advertisement
{
	public class CollectButton : ButtonController
	{
		[SerializeField]
		private AdPanelController _adPanel;

		protected override void OnValidate()
		{
			base.OnValidate();

			_adPanel = GetComponentInParent<AdPanelController>(true);
		}

		protected override void OnButtonClicked()
		{
			_adPanel.Collect();
		}
	}
}
