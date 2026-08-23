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

namespace WoF.Wheel
{
    public class WheelSpin : MonoBehaviour, IStyle<WheelType>
    {
        [SerializeField] 
        private WheelSlotView[] _wheelSlotViews;
        
        [SerializeField]
        private Image _image;

        public Tween PlaySpin(float targetAngle, float duration)
        {
            transform.DOKill();

            return transform.DOLocalRotate(new Vector3(0.0f, 0.0f, targetAngle), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic);
        }

        public void SetRotation(Vector3 rotation)
        {
            transform.localEulerAngles = rotation;
        }

        public Tween PlayBombReaction(float duration)
        {
            return transform.DOPunchRotation(new Vector3(0.0f, 0.0f, 8.0f), duration, 8, 0.5f);
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

        public void ApplySlotView(int index, RewardDefinition reward, int amount)
        {
            _wheelSlotViews[index].SetSprite(reward.Sprite);
            _wheelSlotViews[index].SetAmount(amount);
        }
        
        public void SetActiveWheelSlot(int index, bool active)
        {
            _wheelSlotViews[index].gameObject.SetActive(active);
        }
    }
}
