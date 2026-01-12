using System.Globalization;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IIntParameter : IParameter<int>
    {
    }

    public abstract class AbstractIntParameter : AbstractParameter<int>, IIntParameter
    {
        [SerializeField] private SerializableReactiveProperty<int> _current = new(1000);
        public override ReadOnlyReactiveProperty<int> Current => _current;

        [SerializeField] private SerializableReactiveProperty<int> _min = new(0);
        public override ReadOnlyReactiveProperty<int> Min => _min;

        [SerializeField] private SerializableReactiveProperty<int> _max = new(1000);
        public override ReadOnlyReactiveProperty<int> Max => _max;

        private ReadOnlyReactiveProperty<float> _currentRate;
        public override ReadOnlyReactiveProperty<float> CurrentRate => _currentRate ??= Current
            .CombineLatest(Max, Min, (curr, max, min) =>
                max == min ? 0f : Mathf.Clamp01((float)(curr - min) / (max - min)))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        public override ReadOnlyReactiveProperty<int> CurrentInt => Current;

        private ReadOnlyReactiveProperty<float> _currentFloat;
        public override ReadOnlyReactiveProperty<float> CurrentFloat => _currentFloat ??= Current
            .Select(v => (float)v)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<string> _currentString;
        public override ReadOnlyReactiveProperty<string> CurrentString => _currentString ??= Current
            .Select(v => v.ToString(CultureInfo.CurrentCulture))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isHalfOrLess;
        public override ReadOnlyReactiveProperty<bool> IsHalfOrLess => _isHalfOrLess ??= CurrentRate
            .Select(rate => rate <= 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isAboveHalf;
        public override ReadOnlyReactiveProperty<bool> IsAboveHalf => _isAboveHalf ??= CurrentRate
            .Select(rate => rate > 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);
        
        private ReadOnlyReactiveProperty<bool> _isFull;
        public override ReadOnlyReactiveProperty<bool> IsFull => _isFull ??= Current
            .CombineLatest(Max, (curr, max) => curr >= max)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isEmpty;
        public override ReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty ??= Current
            .CombineLatest(Min, (curr, min) => curr <= min)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        /// <summary>最小値を設定し、現在値を補正</summary>
        public override void SetMin(int min)
        {
            _min.Value = min;
            Refresh();
        }

        /// <summary>最大値を設定し、現在値を補正</summary>
        public override void SetMax(int max)
        {
            _max.Value = max;
            Refresh();
        }

        private void Refresh() => SetClampValue(_current.Value);

        /// <summary>指定値を Min〜Max に制限して設定</summary>
        public override void SetClampValue(int value)
            => _current.Value = Mathf.Clamp(value, _min.Value, _max.Value);

        /// <summary>現在値を加算（Maxを超えない）</summary>
        public override void Add(int value)
        {
            SetClampValue(_current.Value + value);
            _addSubject.OnNext(value);
        }

        /// <summary>現在値を減算（Min未満にならない）</summary>
        public override void Sub(int value)
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