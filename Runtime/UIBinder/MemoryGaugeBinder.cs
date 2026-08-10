using R3;
using TNRD;
using UnityEngine;
using MyUtils.Gauge;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  値をMemoryGaugeにバインドする機能
    /// </summary>
    public class MemoryGaugeBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IRateProvider> _target;

        private void Start()
        {
            if (TryGetComponent<MemoryGauge>(out var gauge))
            {
                _target.Value.CurrentRate
                    .Subscribe(x => gauge.Current.Value = Mathf.CeilToInt(x * gauge.Max))
                    .AddTo(this);
            }
        }
    }
}