using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IFloatParameter : IParameter<float>
    {
    }

    public abstract class AbstractFloatParameter : AbstractNumericParameter<float>, IFloatParameter
    {
        [SerializeField] private SerializableReactiveProperty<float> _current = new(100.0f);
        public override ReadOnlyReactiveProperty<float> Current => _current;

        [SerializeField] private SerializableReactiveProperty<float> _min = new(0.0f);
        public override ReadOnlyReactiveProperty<float> Min => _min;

        [SerializeField] private SerializableReactiveProperty<float> _max = new(100.0f);
        public override ReadOnlyReactiveProperty<float> Max => _max;

        protected override float Plus(float a, float b) => a + b;
        protected override float Minus(float a, float b) => a - b;
        protected override float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);

        protected override float ToRate(float current, float min, float max) =>
            Mathf.Approximately(max, min) ? 0f : Mathf.Clamp01((current - min) / (max - min));

        protected override int ToInt(float value) => Mathf.FloorToInt(value);
        protected override float ToFloat(float value) => value;

        protected override void SetCurrentValue(float value) => _current.Value = value;
        protected override void SetMinValue(float value) => _min.Value = value;
        protected override void SetMaxValue(float value) => _max.Value = value;

        protected override void Awake()
        {
            base.Awake();
            _current.AddTo(this);
            _min.AddTo(this);
            _max.AddTo(this);
        }
    }
}
