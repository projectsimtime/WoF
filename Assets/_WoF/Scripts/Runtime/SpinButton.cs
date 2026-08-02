using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
    public class SpinButton : MonoBehaviour
    {
        [SerializeField]
        private WheelSpin _wheelSpin;

        [SerializeField]
        private Button _button;
        
        // Start is called before the first frame update

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            _wheelSpin.PlaySpin();
        }
    }
}
