using WoF.Core;
using WoF.Session;
using WoF.UI;

namespace WoF.Wheel
{
	public class SpinButton : ButtonController
	{
		private ISpinCommands _spinCommands;

		protected override void OnInject(IServiceResolver services)
		{
			_spinCommands = services.Resolve<ISpinCommands>();
			_spinCommands.SpinningChanged += OnSpinningChanged;

			OnSpinningChanged(_spinCommands.IsSpinning);
		}

		private void OnDestroy()
		{
			if (_spinCommands != null)
			{
				_spinCommands.SpinningChanged -= OnSpinningChanged;
			}
		}

		protected override void OnButtonClicked()
		{
			_spinCommands?.RequestSpin();
		}

		private void OnSpinningChanged(bool isSpinning)
		{
			SetInteractable(!isSpinning);
		}
	}
}
