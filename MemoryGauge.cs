using MyUtils.Parameter;
using R3;
using TNRD;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils
{
    /// <summary>
    /// IParameterのCurrentRateに基づいて、Graphic配列をゲージとして表示するコンポーネント。
    /// </summary>
    public class MemoryGauge : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IParameter> _parameter;

        [Tooltip("ゲージとして表示されるGraphic要素の配列 (例: Image, Text)")]
        [SerializeField] private Graphic[] _memory;

        [Header("色の設定")]
        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _deactivateColor = Color.gray;

        private int _totalMemoryCount;

        private void Start() =>
            _parameter.Value.CurrentRate.Subscribe(UpdateGaugeUI).AddTo(this);

        /// <summary>
        /// パラメータレートに基づいてゲージUIの色を更新します。
        /// </summary>
        /// <param name="rate">現在のレート (0.0f - 1.0f)</param>
        private void UpdateGaugeUI(float rate)
        {
            int activeCount = Mathf.CeilToInt(rate * _memory.Length);

            for (int i = 0; i < _memory.Length; i++)
            {
                var targetColor = i < activeCount ? _activeColor : _deactivateColor;

                if (_memory[i].color != targetColor)
                {
                    _memory[i].color = targetColor;
                }
            }
        }
    }
}