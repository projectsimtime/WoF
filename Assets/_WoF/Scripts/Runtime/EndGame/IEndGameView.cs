using WoF.Core;

namespace WoF.EndGame
{
	public interface IEndGameView : ISceneService
	{
		void Show();

		void Hide();
	}
}
