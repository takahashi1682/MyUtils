using System.Globalization;
using MyUtils.UIBinder;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IParameter
    {
        SerializableReactiveProperty<float> Current { get; }
        SerializableReactiveProperty<float> Min { get; }
        SerializableReactiveProperty<float> Max { get; }
        ReadOnlyReactiveProperty<float> CurrentRate { get; }

        ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; }
        ReadOnlyReactiveProperty<bool> IsAboveHalf { get; }
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
    /// 汎用的なパラメータクラス（HP, Stamina, Gauge など）。
    /// 使用されないReactivePropertyは遅延初期化されます。
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

        // CurrentRate
        private ReadOnlyReactiveProperty<float> _currentRate;
        public ReadOnlyReactiveProperty<float> CurrentRate => _currentRate ??= Current
            .CombineLatest(Max, Min,
                (curr, max, min) => Mathf.Approximately(max, min) ? 0f : Mathf.Clamp01((curr - min) / (max - min)))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // CurrentInt
        private ReadOnlyReactiveProperty<int> _currentInt;
        private ReadOnlyReactiveProperty<int> CurrentIntInternal => _currentInt ??= Current.Select(Mathf.FloorToInt)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // CurrentDouble
        private ReadOnlyReactiveProperty<double> _currentDouble;
        private ReadOnlyReactiveProperty<double> CurrentDoubleInternal => _currentDouble ??= Current
            .Select(v => (double)v)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // CurrentString
        private ReadOnlyReactiveProperty<string> _currentString;
        private ReadOnlyReactiveProperty<string> CurrentStringInternal => _currentString ??= Current
            .Select(v => v.ToString(CultureInfo.CurrentCulture))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // IsHalfOrLess
        private ReadOnlyReactiveProperty<bool> _isHalfOrLess;
        public ReadOnlyReactiveProperty<bool> IsHalfOrLess => _isHalfOrLess ??= CurrentRate
            .Select(rate => rate <= 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // IsAboveHalf
        private ReadOnlyReactiveProperty<bool> _isAboveHalf;
        public ReadOnlyReactiveProperty<bool> IsAboveHalf => _isAboveHalf ??= CurrentRate
            .Select(rate => rate > 0.5f)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // IsFull
        private ReadOnlyReactiveProperty<bool> _isFull;
        public ReadOnlyReactiveProperty<bool> IsFull => _isFull ??= Current
            .CombineLatest(Max, Mathf.Approximately)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // IsEmpty
        private ReadOnlyReactiveProperty<bool> _isEmpty;
        public ReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty ??= Current
            .CombineLatest(Min, Mathf.Approximately)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        // IValueBinder<T>は、privateな遅延初期化Getterを参照する。
        ReadOnlyReactiveProperty<float> IValueBinder<float>.CurrentValue => Current; // floatはCurrentをそのまま使用
        ReadOnlyReactiveProperty<int> IValueBinder<int>.CurrentValue => CurrentIntInternal;
        ReadOnlyReactiveProperty<double> IValueBinder<double>.CurrentValue => CurrentDoubleInternal;
        ReadOnlyReactiveProperty<string> IValueBinder<string>.CurrentValue => CurrentStringInternal;

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

        /// <summary>指定値を Min〜Max に制限して設定</summary>
        public void SetClampValue(float value)
            => Current.Value = Mathf.Clamp(value, Min.Value, Max.Value);

        /// <summary>現在値を加算（Maxを超えない）</summary>
        public void Add(float value) => SetClampValue(Current.Value + value);

        /// <summary>現在値を減算（Min未満にならない）</summary>
        public void Sub(float value) => SetClampValue(Current.Value - value);

        /// <summary>現在値を最大値にする</summary>
        public void SetFull() => Current.Value = Max.Value;

        /// <summary>現在値を最小値にする</summary>
        public void SetEmpty() => Current.Value = Min.Value;
    }
}