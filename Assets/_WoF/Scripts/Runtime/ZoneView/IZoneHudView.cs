using UnityEngine;
using WoF.Core;

namespace WoF.ZoneView
{
	/// <summary>
	/// The "you are here" badge.
	/// </summary>
	public interface IZoneHudView : ISceneService
	{
		void ShowZone(int zoneIndex, Color themeColor);
	}
}
