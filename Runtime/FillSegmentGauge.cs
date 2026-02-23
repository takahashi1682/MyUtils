using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils
{
    /// <summary>
    /// IParameterのCurrentRateに基づいて、Graphic配列をゲージとして表示するコンポーネント。
    /// </summary>
    public class FillSegmentGauge : MonoBehaviour
    {
        public SerializableReactiveProperty<int> Current = new();
        public int OneMemoryMax = 1000;
        [SerializeField] private Image[] _memory = Array.Empty<Image>();

        private void Awake()
        {
            Current.AddTo(this);
            Current.Subscribe(UpdateGaugeUI).AddTo(this);
        }

        /// <summary>
        /// パラメータレートに基づいてゲージUIの色を更新します。
        /// </summary>
        /// <param name="value">現在のレート (0.0f - 1.0f)</param>
        private void UpdateGaugeUI(int value)
        {
            for (int i = 0; i < _memory.Length; i++)
            {
                float diff = value - OneMemoryMax * i;
                float rate = diff / OneMemoryMax;
                _memory[i].fillAmount = Mathf.Clamp01(rate);
            }
        }
    }
}