using System;
using MyUtils.AudioManager.Manager;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MyUtils.UICommon.UISelectable.UISliderChange
{
    /// <summary>
    /// Sliderが変更された時に音を再生する機能
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class UISliderChangeSoundPlayer : MonoBehaviour
    {
        public AudioResource Resource;
        public float Interval = 0.3f;

        private void Start()
        {
            if (TryGetComponent(out Slider slider))
            {
                slider.OnValueChangedAsObservable()
                    .ThrottleFirstLast(
                        TimeSpan.FromSeconds(Interval),
                        UnityTimeProvider.TimeUpdateRealtime)
                    .Subscribe(_ => SEManager.Play(Resource))
                    .AddTo(this);
            }
        }
    }
}