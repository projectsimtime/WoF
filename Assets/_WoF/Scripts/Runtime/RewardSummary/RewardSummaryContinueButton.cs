using System;
using WoF.UI;

namespace WoF.RewardSummary
{
	public class RewardSummaryContinueButton : ButtonController
	{
		public event Action ContinueClicked;

		protected override void OnButtonClicked()
		{
			ContinueClicked?.Invoke();
		}
	}
}
