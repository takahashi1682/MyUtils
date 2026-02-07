using R3;
using TNRD;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  値をスライダーにバインドする機能
    /// </summary>
    public class SliderBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IRateBinder> _target;

        private void Start()
        {
            if (TryGetComponent<Slider>(out var slider))
            {
                _target.Value.CurrentRate
                    .Subscribe(x => slider.value = x)
                    .AddTo(this);
            }
        }
    }
}