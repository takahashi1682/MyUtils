using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IIntParameter : IParameter<int>
    {
    }

    public abstract class AbstractIntParameter : AbstractNumericParameter<int>, IIntParameter
    {
        [SerializeField] private SerializableReactiveProperty<int> _current = new(1000);
        public override ReadOnlyReactiveProperty<int> Current => _current;

        [SerializeField] private SerializableReactiveProperty<int> _min = new(0);
        public override ReadOnlyReactiveProperty<int> Min => _min;

        [SerializeField] private SerializableReactiveProperty<int> _max = new(1000);
        public override ReadOnlyReactiveProperty<int> Max => _max;

        protected override int Plus(int a, int b) => a + b;
        protected override int Minus(int a, int b) => a - b;
        protected override int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);

        protected override float ToRate(int current, int min, int max) =>
            max == min ? 0f : Mathf.Clamp01((float)(current - min) / (max - min));

        protected override int ToInt(int value) => value;
        protected override float ToFloat(int value) => value;

        protected override void SetCurrentValue(int value) => _current.Value = value;
        protected override void SetMinValue(int value) => _min.Value = value;
        protected override void SetMaxValue(int value) => _max.Value = value;

        protected override void Awake()
        {
            base.Awake();
            _current.AddTo(this);
            _min.AddTo(this);
            _max.AddTo(this);
        }
    }
}
