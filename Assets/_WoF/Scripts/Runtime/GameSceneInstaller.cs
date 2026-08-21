using UnityEngine;
using WoF.Wheel;

namespace WoF
{
    public sealed class GameSceneInstaller : MonoBehaviour
    {
        [SerializeField]
        private SpinButton _spinButton;

        [SerializeField]
        private GameSession _gameSession;

        private void OnEnable()
        {
            if (_spinButton == null || _gameSession == null)
            {
                Debug.LogError("GameSceneInstaller spin references are not assigned.", this);
                return;
            }

            _spinButton.Clicked += _gameSession.OnSpinClicked;
            _gameSession.WheelSpinningChanged += _spinButton.SetSpinning;
        }

        private void OnDisable()
        {
            if (_spinButton == null || _gameSession == null)
            {
                return;
            }

            _spinButton.Clicked -= _gameSession.OnSpinClicked;
            _gameSession.WheelSpinningChanged -= _spinButton.SetSpinning;
        }
    }
}
