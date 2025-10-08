using System.Collections.Generic;
using R3;
using TMPro;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class RateToTextBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IRateBinder> _target;

        [Header("未指定時は同じGameObjectのTextMeshProUGUIを使用")]
        [SerializeField] private TextMeshProUGUI _text;

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
            // 0~1のRateからインデックス計算
            int index = Mathf.FloorToInt(rateValue * _messages.Count);
            index = Mathf.Clamp(index, 0, _messages.Count - 1);
            _text.text = _messages[index];
        }
    }
}