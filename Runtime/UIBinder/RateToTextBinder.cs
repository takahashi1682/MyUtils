using System.Collections.Generic;
using MyUtils.Abstract;
using R3;
using TMPro;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    public class RateToTextBinder : AbstractTargetBehaviour<TextMeshProUGUI>
    {
        [SerializeField] private SerializableInterface<IRateProvider> _inRate;

        [Header("Rateを0~100で分割して表示するメッセージ")]
        [SerializeField] private List<string> _messages;

        /// <summary>値の供給元。既定はInspectorで割り当てた参照。DIなどで差し替える場合はオーバーライドする。</summary>
        protected virtual IRateProvider ResolveProvider() => _inRate.Value;

        protected override void Start()
        {
            base.Start();
            ResolveProvider().CurrentRate
                .Subscribe(UpdateText)
                .AddTo(this);
        }

        private void UpdateText(float rateValue)
        {
            // 0~1のRateからインデックス計算
            int index = Mathf.FloorToInt(rateValue * _messages.Count);
            index = Mathf.Clamp(index, 0, _messages.Count - 1);
            Target.text = _messages[index];
        }
    }
}