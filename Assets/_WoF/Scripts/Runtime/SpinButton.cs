using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
    public class SpinButton : ButtonController
    {
        protected override void OnButtonClicked()
        {
            var tween = _gameSession.OnSpinClicked();
            Button.interactable = false;
            
            tween.onComplete += OnSpinCompleted;
        }

        private void OnSpinCompleted()
        {
            Button.interactable = true;
        }
    }
}
