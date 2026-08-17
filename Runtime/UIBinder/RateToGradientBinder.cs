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
        [SerializeField] private SerializableInterface<IRateProvider> _target;
        [SerializeField] private Gradient _gradient;

        protected override void Start()
        {
            base.Start();
            _target.Value.CurrentRate.Subscribe(x => Target.color = _gradient.Evaluate(x)).AddTo(this);
        }
    }
}