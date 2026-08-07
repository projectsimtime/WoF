using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WoF
{
    public class SpinButton : ButtonController
    {
        protected override void OnButtonClicked()
        {
            Tween tween = _gameSession.OnSpinClicked();
            SetButtonInteractable(false);
            
            tween.onComplete += OnSpinCompleted;
        }

        private void OnSpinCompleted()
        {
            SetButtonInteractable(true);
        }
    }
}
