using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WoF.Interface;

namespace WoF
{
    public class WheelSpin : MonoBehaviour, IStyle<WheelType>
    {
        private int FullSpinCount = 10;

        private float FullSpinDuration = 10.0f;

        [SerializeField] 
        private WheelSlotView[] _wheelSlotViews;
        
        [SerializeField]
        private Image _image;

        public void PlaySpin()
        {
            transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 360.0f * FullSpinCount)/*transform.eulerAngles + new Vector3(0.0f, 0.0f, 90.0f)*/, FullSpinDuration, RotateMode.FastBeyond360);
        }

        private void OnValidate()
        {
            _wheelSlotViews = GetComponentsInChildren<WheelSlotView>();
            
            _image = GetComponent<Image>();
        }

        public void ApplyStyle(WheelType style)
        {
            _image.sprite = style.WheelSprite;
        }
    }
}
