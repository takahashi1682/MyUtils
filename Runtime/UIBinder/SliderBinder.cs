using MyUtils.Abstract;
using R3;
using TNRD;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  値をスライダーにバインドする機能
    /// </summary>
    public class SliderBinder : AbstractTargetBehaviour<Slider>
    {
        [SerializeField] protected SerializableInterface<IRateProvider> _inRate;

        /// <summary>値の供給元。既定はInspectorで割り当てた参照。DIなどで差し替える場合はオーバーライドする。</summary>
        protected virtual IRateProvider ResolveProvider() => _inRate.Value;

        protected override void Start()
        {
            base.Start();
            ResolveProvider().CurrentRate
                .Subscribe(x => Target.value = x)
                .AddTo(this);
        }
    }
}