using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoF.Wheel
{
    [RequireComponent(typeof(Button))]
    public sealed class SpinButton : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        public event Action Clicked;

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        private void Awake()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            Clicked?.Invoke();
        }

        public void SetSpinning(bool isSpinning)
        {
            _button.interactable = !isSpinning;
        }
    }
}
