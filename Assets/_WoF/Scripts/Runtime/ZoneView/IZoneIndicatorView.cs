using System.Collections.Generic;
using WoF.Core;

namespace WoF.ZoneView
{
	/// <summary>
	/// The bar of upcoming milestone zones. It takes a list rather than a fixed
	/// safe-zone / super-zone pair, so a new milestone flavour needs no change here.
	/// </summary>
	public interface IZoneIndicatorView : ISceneService
	{
		void ShowMilestones(IReadOnlyList<ZoneMilestoneData> milestones);
	}
}
