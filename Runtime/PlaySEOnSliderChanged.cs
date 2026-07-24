using System;
using MyUtils.AudioManager.Manager;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils
{
    /// <summary>
    /// Sliderが変更された時に音を再生する機能
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class PlaySEOnSliderChanged : MonoBehaviour
    {
        public AudioClip Clip;
        public float Interval = 0.3f;

        private void Start()
        {
            if (TryGetComponent(out Slider slider))
            {
                slider.OnValueChangedAsObservable()
                    .ThrottleFirstLast(
                        TimeSpan.FromSeconds(Interval),
                        UnityTimeProvider.TimeUpdateRealtime)
                    .Subscribe(_ => SEManager.Play(Clip))
                    .AddTo(this);
            }
        }
    }
}