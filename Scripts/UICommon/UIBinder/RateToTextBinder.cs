using System.Collections.Generic;
using R3;
using TMPro;
using TNRD;
using UnityEngine;

namespace TUtils.UICommon.UIBinder
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class RateToTextBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IRateBinder> _target;

        [Header("未指定時は同じGameObjectのTextMeshProUGUIを使用")]
        [SerializeField] private TextMeshProUGUI _text;

        [Header("Rateが0または最大値のとき専用のセリフ")]
        [SerializeField] private string _zeroMessage = "最低値";
        [SerializeField] private string _maxMessage = "最大値";

        [Header("Rateを0~100で分割して表示するメッセージ")]
        [SerializeField] private List<string> _messages;

        private void Awake()
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _target.Value.CurrentRate
                .Subscribe(UpdateText)
                .AddTo(this);
        }

        private void UpdateText(float rateValue)
        {
            if (_messages == null || _messages.Count == 0) return;

            switch (rateValue)
            {
                case <= 0f:
                    _text.text = _zeroMessage;
                    break;

                case >= 1f:
                    _text.text = _maxMessage;
                    break;

                default:
                    // 0~1のRateからインデックス計算
                    int index = Mathf.FloorToInt(rateValue * _messages.Count);
                    index = Mathf.Clamp(index, 0, _messages.Count - 1);
                    _text.text = _messages[index];
                    break;
            }
        }
    }
}