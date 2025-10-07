using System.Globalization;
using Cysharp.Threading.Tasks;
using MyUtils.UICommon.UIBinder;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IParameter
    {
        SerializableReactiveProperty<float> Max { get; }
        SerializableReactiveProperty<float> Current { get; }
        SerializableReactiveProperty<float> Min { get; }
        ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; }
        ReadOnlyReactiveProperty<bool> IsFull { get; }
        ReadOnlyReactiveProperty<bool> IsEmpty { get; }

        void SetMax(float max);
        void SetMin(float min);
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
        [field: SerializeField] public SerializableReactiveProperty<float> Max { get; private set; } = new(1000f);
        [field: SerializeField] public SerializableReactiveProperty<float> Min { get; private set; } = new(0f);

        public ReadOnlyReactiveProperty<int> CurrentInt;
        public ReadOnlyReactiveProperty<float> CurrentFloat;
        public ReadOnlyReactiveProperty<double> CurrentDouble;
        public ReadOnlyReactiveProperty<string> CurrentString;
        ReadOnlyReactiveProperty<int> IValueBinder<int>.CurrentValue => CurrentInt;
        ReadOnlyReactiveProperty<float> IValueBinder<float>.CurrentValue => CurrentFloat;
        ReadOnlyReactiveProperty<double> IValueBinder<double>.CurrentValue => CurrentDouble;
        ReadOnlyReactiveProperty<string> IValueBinder<string>.CurrentValue => CurrentString;
        public ReadOnlyReactiveProperty<float> CurrentRate { get; private set; }

        public ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsFull { get; private set; }
        public ReadOnlyReactiveProperty<bool> IsEmpty { get; private set; }

        protected virtual void Awake()
        {
            CurrentInt = Current.Select(Mathf.FloorToInt)
                .ToReadOnlyReactiveProperty()
                .AddTo(this);

            CurrentFloat = Current
                .ToReadOnlyReactiveProperty()
                .AddTo(this);

            CurrentDouble = Current
                .Select(v => (double)v)
                .ToReadOnlyReactiveProperty()
                .AddTo(this);

            CurrentString = Current
                .Select(v => v.ToString(CultureInfo.CurrentCulture))
                .ToReadOnlyReactiveProperty()
                .AddTo(this);

            CurrentRate = Current
                .CombineLatest(Max, Min,
                    (curr, max, min) => max == 0f ? 0f : Mathf.Clamp01((curr - min) / (max - min)))
                .ToReadOnlyReactiveProperty()
                .AddTo(this);

            IsHalfOrLess = CurrentRate
                .Select(rate => rate <= 0.5f)
                .ToReadOnlyReactiveProperty()
                .AddTo(this);

            IsFull = Current
                .CombineLatest(Max, Mathf.Approximately)
                .ToReadOnlyReactiveProperty()
                .AddTo(this);

            IsEmpty = Current
                .CombineLatest(Min, Mathf.Approximately)
                .ToReadOnlyReactiveProperty()
                .AddTo(this);
        }

        /// <summary>最大値を設定し、現在値を補正</summary>
        public void SetMax(float max)
        {
            Max.Value = max;
            SetClampValue(Current.Value);
        }

        /// <summary>最小値を設定し、現在値を補正</summary>
        public void SetMin(float min)
        {
            Min.Value = min;
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
        public void SetEmpty() => Current.Value = 0;
    }
}