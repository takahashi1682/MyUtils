using R3;
using TNRD;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  Rateをグラデーション評価した色にバインドする機能
    /// </summary>
    public class RateToGradientBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IRateProvider> _target;
        [SerializeField] private Gradient _gradient;

        private void Start()
        {
            if (TryGetComponent<Graphic>(out var graphic))
            {
                _target.Value.CurrentRate.Subscribe(x => graphic.color = _gradient.Evaluate(x)).AddTo(this);
            }
        }
    }
}