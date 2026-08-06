using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using WoF.Interface;
using WoF.Reward;

namespace WoF
{
    public class WheelSpin : MonoBehaviour, IStyle<WheelType>
    {
        [SerializeField] 
        private WheelSlotView[] _wheelSlotViews;
        
        [SerializeField]
        private Image _image;

        public TweenerCore<Quaternion, Vector3, QuaternionOptions> PlaySpin(float targetAngle, float duration)
        {
            return transform.DOLocalRotate(new Vector3(0.0f, 0.0f, targetAngle), duration, RotateMode.FastBeyond360);
        }

        public void SetRotation(Vector3 rotation)
        {
            transform.eulerAngles = rotation;
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

        public void ApplySlotView(int index, RewardDefinition reward)
        {
            _wheelSlotViews[index].SetSprite(reward.Sprite);
        }
        
        public void SetActiveWheelSlot(int index, bool active)
        {
            _wheelSlotViews[index].gameObject.SetActive(active);
        }
    }
}
