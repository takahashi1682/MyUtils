using R3;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  値をスライダーにバインドする機能
    /// </summary>
    public class MemoryBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IRateBinder> _target;

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