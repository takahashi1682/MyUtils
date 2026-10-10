using MyUtils.Abstract;
using R3;
using TNRD;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  Rateをグラデーション評価した色にバインドする機能
    /// </summary>
    public class RateToGradientBinder : AbstractTargetBehaviour<Graphic>
    {
        [SerializeField] private SerializableInterface<IRateProvider> _inRate;
        [SerializeField] private Gradient _gradient;

        /// <summary>値の供給元。既定はInspectorで割り当てた参照。DIなどで差し替える場合はオーバーライドする。</summary>
        protected virtual IRateProvider ResolveProvider() => _inRate.Value;

        protected override void Start()
        {
            base.Start();
            ResolveProvider().CurrentRate.Subscribe(x => Target.color = _gradient.Evaluate(x)).AddTo(this);
        }
    }
}