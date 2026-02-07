using System.Globalization;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IFloatParameter : IParameter<float>
    {
    }

    public abstract class AbstractFloatParameter : AbstractParameter<float>, IFloatParameter
    {
        [SerializeField] protected SerializableReactiveProperty<float> _current = new(100.0f);
        public override ReadOnlyReactiveProperty<float> Current => _current;

        [SerializeField] protected SerializableReactiveProperty<float> _min = new(0.0f);
        public override ReadOnlyReactiveProperty<float> Min => _min;

        [SerializeField] protected SerializableReactiveProperty<float> _max = new(100.0f);
        public override ReadOnlyReactiveProperty<float> Max => _max;

        protected ReadOnlyReactiveProperty<float> _currentRate;
        public override ReadOnlyReactiveProperty<float> CurrentRate => _currentRate ??= Current
            .CombineLatest(Max, Min, (curr, max, min) =>
                Mathf.Approximately(max, min) ? 0f : Mathf.Clamp01((curr - min) / (max - min)))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        protected ReadOnlyReactiveProperty<int> _currentInt;
        public override ReadOnlyReactiveProperty<int> CurrentInt => _currentInt ??= Current
            .Select(Mathf.FloorToInt)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        public override ReadOnlyReactiveProperty<float> CurrentFloat => Current;

        protected ReadOnlyReactiveProperty<string> _currentString;
        public override ReadOnlyReactiveProperty<string> CurrentString => _currentString ??= Current
            .Select(v => v.ToString(CultureInfo.CurrentCulture))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        protected ReadOnlyReactiveProperty<bool> _isHalfOrLess;
        public override ReadOnlyReactiveProperty<bool> IsHalfOrLess => _isHalfOrLess ??= CurrentRate
            .Select(rate => rate <= 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        protected ReadOnlyReactiveProperty<bool> _isAboveHalf;
        public override ReadOnlyReactiveProperty<bool> IsAboveHalf => _isAboveHalf ??= CurrentRate
            .Select(rate => rate > 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        protected ReadOnlyReactiveProperty<bool> _isFull;
        public override ReadOnlyReactiveProperty<bool> IsFull => _isFull ??= Current
            .CombineLatest(Max, (curr, max) => curr >= max || Mathf.Approximately(curr, max))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        protected ReadOnlyReactiveProperty<bool> _isEmpty;
        public override ReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty ??= Current
            .CombineLatest(Min, (curr, min) => curr <= min || Mathf.Approximately(curr, min))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        /// <summary>最小値を設定し、現在値を補正</summary>
        public override void SetMin(float min)
        {
            _min.Value = min;
            Refresh();
        }

        /// <summary>最大値を設定し、現在値を補正</summary>
        public override void SetMax(float max)
        {
            _max.Value = max;
            Refresh();
        }

        protected void Refresh() => SetClampValue(_current.Value);

        /// <summary>指定値を Min〜Max に制限して設定</summary>
        public override void SetClampValue(float value)
            => _current.Value = Mathf.Clamp(value, _min.Value, _max.Value);

        /// <summary>現在値を加算（Maxを超えない）</summary>
        public override void Add(float value)
        {
            SetClampValue(_current.Value + value);
            _addSubject.OnNext(value);
        }

        /// <summary>現在値を減算（Min未満にならない）</summary>
        public override void Sub(float value)
        {
            SetClampValue(_current.Value - value);
            _subSubject.OnNext(value);
        }

        /// <summary>現在値を最大値にする</summary>
        public override void SetFull() => _current.Value = _max.Value;

        /// <summary>現在値を最小値にする</summary>
        public override void SetEmpty() => _current.Value = _min.Value;

        protected override void Awake()
        {
            base.Awake();
            _current.AddTo(this);
            _min.AddTo(this);
            _max.AddTo(this);
        }
    }
}