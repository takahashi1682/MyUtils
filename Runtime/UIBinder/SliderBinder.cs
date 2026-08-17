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
        [SerializeField] private SerializableInterface<IRateProvider> _target;

        protected override void Start()
        {
            base.Start();
            _target.Value.CurrentRate
                .Subscribe(x => Target.value = x)
                .AddTo(this);
        }
    }
}