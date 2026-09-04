using MyUtils.Abstract;
using R3;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    /// <summary>
    /// 値をGaugeにバインドする機能
    /// </summary>
    public class GaugeBinder : AbstractTargetBehaviour<UI.Gauge>
    {
        [SerializeField] private SerializableInterface<IRateProvider> _inRate;

        protected override void Start()
        {
            base.Start();
            _inRate.Value.CurrentRate
                .Subscribe(x => Target.Value = x)
                .AddTo(this);
        }
    }
}