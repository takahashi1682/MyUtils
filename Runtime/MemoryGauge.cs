using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils
{
    /// <summary>
    /// IParameterのCurrentRateに基づいて、Graphic配列をゲージとして表示するコンポーネント。
    /// </summary>
    public class MemoryGauge : MonoBehaviour
    {
        public SerializableReactiveProperty<int> Current = new();
        public int Max = 5;
        [SerializeField] private Image[] _memory = Array.Empty<Image>();

        [Header("色の設定")]
        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _deactivateColor = new Color32(0, 0, 0, 200);

        private void Awake()
        {
            Current.AddTo(this);
            Current.Select(x => Math.Clamp(x, 0, Max)).Subscribe(UpdateGaugeUI).AddTo(this);
        }

        /// <summary>
        /// パラメータレートに基づいてゲージUIの色を更新します。
        /// </summary>
        /// <param name="value">現在のレート (0.0f - 1.0f)</param>
        private void UpdateGaugeUI(int value)
        {
            for (int i = 0; i < _memory.Length; i++)
            {
                var targetColor = i < value ? _activeColor : _deactivateColor;

                if (_memory[i].color != targetColor)
                {
                    _memory[i].color = targetColor;
                }
            }
        }
    }
}