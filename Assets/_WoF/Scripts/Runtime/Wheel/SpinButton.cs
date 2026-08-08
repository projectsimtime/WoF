using WoF.UI;

namespace WoF.Wheel
{
    public class SpinButton : ButtonController
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            _gameSession.WheelSpinningChanged += OnWheelSpinningChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _gameSession.WheelSpinningChanged -= OnWheelSpinningChanged;
        }

        protected override void OnButtonClicked()
        {
            _gameSession.OnSpinClicked();
        }

        private void OnWheelSpinningChanged(bool isSpinning)
        {
            SetButtonInteractable(!isSpinning);
        }
    }
}
