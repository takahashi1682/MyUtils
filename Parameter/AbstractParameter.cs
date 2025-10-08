using System.Globalization;
using MyUtils.UIBinder;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IParameter
    {
        SerializableReactiveProperty<float> Current { get; }
        ReadOnlyReactiveProperty<int> CurrentInt { get; }
        ReadOnlyReactiveProperty<double> CurrentDouble { get; }
        ReadOnlyReactiveProperty<string> CurrentString { get; }
        SerializableReactiveProperty<float> Min { get; }
        SerializableReactiveProperty<float> Max { get; }
        ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; }
        ReadOnlyReactiveProperty<bool> IsHalfOrThan { get; }
        ReadOnlyReactiveProperty<bool> IsFull { get; }
        ReadOnlyReactiveProperty<bool> IsEmpty { get; }

        void SetMin(float min);
        void SetMax(float max);
        void SetClampValue(float value);
        void Add(float value);
        void Sub(float value);
        void SetFull();
        void SetEmpty();
    }

    /// <summary>
    /// 汎用的なパラメータクラス（HP, Stamina, Gauge など）
    /// </summary>
    public abstract class AbstractParameter : MonoBehaviour,
        IParameter,
        IRateBinder,
        IViewSwitchBinder,
        IValueBinder<int>,
        IValueBinder<float>,
        IValueBinder<double>,
        IValueBinder<string>
    {
        [field: SerializeField] public SerializableReactiveProperty<float> Current { get; private set; } = new(1000f);
        [field: SerializeField] public SerializableReactiveProperty<float> Min { get; private set; } = new(0f);
        [field: SerializeField] public SerializableReactiveProperty<float> Max { get; private set; } = new(1000f);

        private ReadOnlyReactiveProperty<int> _currentInt;
        public ReadOnlyReactiveProperty<int> CurrentInt => _currentInt ??= Current.Select(Mathf.FloorToInt)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);
        ReadOnlyReactiveProperty<int> IValueBinder<int>.CurrentValue => CurrentInt;


        ReadOnlyReactiveProperty<float> IValueBinder<float>.CurrentValue => Current;

        private ReadOnlyReactiveProperty<double> _currentDouble;

        public ReadOnlyReactiveProperty<double> CurrentDouble => _currentDouble ??= Current
            .Select(v => (double)v)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);
        ReadOnlyReactiveProperty<double> IValueBinder<double>.CurrentValue => CurrentDouble;

        private ReadOnlyReactiveProperty<string> _currentString;
        public ReadOnlyReactiveProperty<string> CurrentString => _currentString ??= Current
            .Select(v => v.ToString(CultureInfo.CurrentCulture))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);
        ReadOnlyReactiveProperty<string> IValueBinder<string>.CurrentValue => CurrentString;

        private ReadOnlyReactiveProperty<float> _currentRate;
        public ReadOnlyReactiveProperty<float> CurrentRate => _currentRate ??= Current
            .CombineLatest(Max, Min,
                (curr, max, min) => Mathf.Approximately(max, min) ? 0f : Mathf.Clamp01((curr - min) / (max - min)))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isHalfOrLess;
        public ReadOnlyReactiveProperty<bool> IsHalfOrLess => _isHalfOrLess ??= CurrentRate
            .Select(rate => rate <= 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isHalfOrThan;
        public ReadOnlyReactiveProperty<bool> IsHalfOrThan => _isHalfOrThan ??= CurrentRate
            .Select(rate => rate >= 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isFull;
        public ReadOnlyReactiveProperty<bool> IsFull => _isFull ??= Current
            .CombineLatest(Max, Mathf.Approximately)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isEmpty;
        public ReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty ??= Current
            .CombineLatest(Min, Mathf.Approximately)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        /// <summary>最小値を設定し、現在値を補正</summary>
        public void SetMin(float min)
        {
            Min.Value = min;
            SetClampValue(Current.Value);
        }

        /// <summary>最大値を設定し、現在値を補正</summary>
        public void SetMax(float max)
        {
            Max.Value = max;
            SetClampValue(Current.Value);
        }

        /// <summary>指定値を 0〜最大値に制限して設定</summary>
        public void SetClampValue(float value)
            => Current.Value = Mathf.Clamp(value, Min.Value, Max.Value);

        /// <summary>現在値を加算（最大値を超えない）</summary>
        public void Add(float value) => SetClampValue(Current.Value + value);

        /// <summary>現在値を減算（0未満にならない）</summary>
        public void Sub(float value) => SetClampValue(Current.Value - value);

        /// <summary>現在値を最大値にする</summary>
        public void SetFull() => Current.Value = Max.Value;

        /// <summary>現在値を0にする</summary>
        public void SetEmpty() => Current.Value = Min.Value; // 0ではなくMin.Valueが適切
    }
}