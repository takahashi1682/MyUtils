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

        protected override void Start()
        {
            base.Start();
            _inRate.Value.CurrentRate
                .Subscribe(x => Target.Current.Value = Mathf.CeilToInt(x * Target.Max))
                .AddTo(this);
        }
    }
}