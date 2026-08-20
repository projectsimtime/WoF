using WoF.Core;

namespace WoF.ExitPanel
{
	public interface IExitView : ISceneService
	{
		void Show();

		void Hide();
	}
}
