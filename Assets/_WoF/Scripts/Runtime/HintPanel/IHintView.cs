using WoF.Core;
using WoF.Reward;

namespace WoF.HintPanel
{
	public interface IHintView : ISceneService
	{
		void Show();

		void Hide();

		void ShowHint(RewardDefinition reward, int amount);
	}
}
