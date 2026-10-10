using MyUtils.Abstract;
using MyUtils.Gauge;
using R3;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  値をMemoryGaugeにバインドする機能
    /// </summary>
    public class MemoryGaugeBinder : AbstractTargetBehaviour<MemoryGauge>
    {
        [SerializeField] private SerializableInterface<IRateProvider> _inRate;

        /// <summary>値の供給元。既定はInspectorで割り当てた参照。DIなどで差し替える場合はオーバーライドする。</summary>
        protected virtual IRateProvider ResolveProvider() => _inRate.Value;

        protected override void Start()
        {
            base.Start();
            ResolveProvider().CurrentRate
                .Subscribe(x => Target.Current.Value = Mathf.CeilToInt(x * Target.Max))
                .AddTo(this);
        }
    }
}