using System;
using System.Globalization;
using R3;

namespace MyUtils.Parameter
{
    /// <summary>
    /// 数値型パラメータの共通実装。
    /// CurrentRate/CurrentInt/CurrentFloat/CurrentString/IsHalfOrLess/IsAboveHalf/IsFull/IsEmpty や
    /// SetMin/SetMax/SetClampValue/Add/Sub/SetFull/SetEmpty をここで一括実装し、
    /// サブクラスは型固有の演算（Plus/Minus/Clamp/変換/値の書き込み）のみを実装すればよい。
    /// </summary>
    public abstract class AbstractNumericParameter<T> : AbstractParameter<T> where T : struct, IComparable<T>, IFormattable
    {
        private ReadOnlyReactiveProperty<float> _currentRate;
        public override ReadOnlyReactiveProperty<float> CurrentRate => _currentRate ??= Current
            .CombineLatest(Max, Min, (curr, max, min) => ToRate(curr, min, max))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<int> _currentInt;
        public override ReadOnlyReactiveProperty<int> CurrentInt => _currentInt ??= Current
            .Select(ToInt)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<float> _currentFloat;
        public override ReadOnlyReactiveProperty<float> CurrentFloat => _currentFloat ??= Current
            .Select(ToFloat)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<string> _currentString;
        public override ReadOnlyReactiveProperty<string> CurrentString => _currentString ??= Current
            .Select(v => v.ToString(null, CultureInfo.CurrentCulture))
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
            .CombineLatest(Max, (curr, max) => curr.CompareTo(max) >= 0)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isEmpty;
        public override ReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty ??= Current
            .CombineLatest(Min, (curr, min) => curr.CompareTo(min) <= 0)
            .ToReadOnlyReactiveProperty()
            .AddTo(this);

        /// <summary>最小値を設定し、現在値を補正</summary>
        public override void SetMin(T min)
        {
            SetMinValue(min);
            Refresh();
        }

        /// <summary>最大値を設定し、現在値を補正</summary>
        public override void SetMax(T max)
        {
            SetMaxValue(max);
            Refresh();
        }

        private void Refresh() => SetClampValue(Current.CurrentValue);

        /// <summary>指定値を Min〜Max に制限して設定</summary>
        public override void SetClampValue(T value)
            => SetCurrentValue(Clamp(value, Min.CurrentValue, Max.CurrentValue));

        /// <summary>現在値を加算（Maxを超えない）</summary>
        public override void Add(T value)
        {
            SetClampValue(Plus(Current.CurrentValue, value));
            _addSubject.OnNext(value);
        }

        /// <summary>現在値を減算（Min未満にならない）</summary>
        public override void Sub(T value)
        {
            SetClampValue(Minus(Current.CurrentValue, value));
            _subSubject.OnNext(value);
        }

        /// <summary>現在値を最大値にする</summary>
        public override void SetFull() => SetCurrentValue(Max.CurrentValue);

        /// <summary>現在値を最小値にする</summary>
        public override void SetEmpty() => SetCurrentValue(Min.CurrentValue);

        /// <summary>a + b を返す</summary>
        protected abstract T Plus(T a, T b);

        /// <summary>a - b を返す</summary>
        protected abstract T Minus(T a, T b);

        /// <summary>valueをmin〜maxに制限する</summary>
        protected abstract T Clamp(T value, T min, T max);

        /// <summary>current/min/maxから0〜1の割合を算出する（min == maxの場合は0）</summary>
        protected abstract float ToRate(T current, T min, T max);

        /// <summary>intへの変換</summary>
        protected abstract int ToInt(T value);

        /// <summary>floatへの変換</summary>
        protected abstract float ToFloat(T value);

        /// <summary>現在値の書き込み（バッキングフィールドはサブクラスが保持）</summary>
        protected abstract void SetCurrentValue(T value);

        /// <summary>最小値の書き込み（バッキングフィールドはサブクラスが保持）</summary>
        protected abstract void SetMinValue(T value);

        /// <summary>最大値の書き込み（バッキングフィールドはサブクラスが保持）</summary>
        protected abstract void SetMaxValue(T value);
    }
}
