using MyUtils.UIBinder;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IParameter<T>
    {
        T CurrentValue { get; }

        T MinValue { get; }

        T MaxValue { get; }

        /// <summary>現在値</summary>
        ReadOnlyReactiveProperty<T> Current { get; }

        /// <summary>最小値</summary>
        ReadOnlyReactiveProperty<T> Min { get; }

        /// <summary>最大値</summary>
        ReadOnlyReactiveProperty<T> Max { get; }

        /// <summary>Min〜Maxの範囲を0〜1に正規化した割合</summary>
        ReadOnlyReactiveProperty<float> CurrentRate { get; }

        /// <summary>現在値をintとして取得</summary>
        ReadOnlyReactiveProperty<int> CurrentInt { get; }

        /// <summary>現在値をfloatとして取得</summary>
        ReadOnlyReactiveProperty<float> CurrentFloat { get; }

        /// <summary>現在値をstringとして取得</summary>
        ReadOnlyReactiveProperty<string> CurrentString { get; }

        /// <summary>割合が50%以下かどうか</summary>
        ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; }

        /// <summary>割合が50%より大きいかどうか</summary>
        ReadOnlyReactiveProperty<bool> IsAboveHalf { get; }

        /// <summary>Add実行時に加算量を通知</summary>
        Observable<T> OnAdd { get; }

        /// <summary>Sub実行時に減算量を通知</summary>
        Observable<T> OnSub { get; }

        /// <summary>現在値が最大値に達しているか</summary>
        ReadOnlyReactiveProperty<bool> IsFull { get; }

        /// <summary>現在値が最小値まで下がっているか</summary>
        ReadOnlyReactiveProperty<bool> IsEmpty { get; }

        /// <summary>最小値を設定し、現在値を補正</summary>
        void SetMin(T min);

        /// <summary>最大値を設定し、現在値を補正</summary>
        void SetMax(T max);

        /// <summary>指定値を Min〜Max に制限して設定</summary>
        void SetClampValue(T value);

        /// <summary>現在値を加算（Maxを超えない）</summary>
        void Add(T value);

        /// <summary>現在値を減算（Min未満にならない）</summary>
        void Sub(T value);

        /// <summary>現在値を最大値にする</summary>
        void SetFull();

        /// <summary>現在値を最小値にする</summary>
        void SetEmpty();
    }

    /// <summary>
    /// 汎用的なパラメータクラス（HP, Stamina, Gauge など）。
    /// 使用されないReactivePropertyは遅延初期化されます。
    /// </summary>
    public abstract class AbstractParameter<T> : MonoBehaviour,
        IParameter<T>,
        IRateProvider,
        IViewSwitchProvider,
        IValueProvider<int>,
        IValueProvider<float>,
        IValueProvider<string>
    {
        public T CurrentValue => Current.CurrentValue;
        public T MinValue => Min.CurrentValue;
        public T MaxValue => Max.CurrentValue;
        
        public abstract ReadOnlyReactiveProperty<T> Current { get; }
        public abstract ReadOnlyReactiveProperty<T> Min { get; }
        public abstract ReadOnlyReactiveProperty<T> Max { get; }

        public abstract ReadOnlyReactiveProperty<float> CurrentRate { get; }

        public abstract ReadOnlyReactiveProperty<int> CurrentInt { get; }
        public abstract ReadOnlyReactiveProperty<float> CurrentFloat { get; }
        public abstract ReadOnlyReactiveProperty<string> CurrentString { get; }

        ReadOnlyReactiveProperty<int> IValueProvider<int>.CurrentValue => CurrentInt;
        ReadOnlyReactiveProperty<float> IValueProvider<float>.CurrentValue => CurrentFloat;
        ReadOnlyReactiveProperty<string> IValueProvider<string>.CurrentValue => CurrentString;

        public abstract ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsAboveHalf { get; }

        protected readonly Subject<T> _addSubject = new();
        public Observable<T> OnAdd => _addSubject;

        protected readonly Subject<T> _subSubject = new();
        public Observable<T> OnSub => _subSubject;

        public abstract ReadOnlyReactiveProperty<bool> IsFull { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsEmpty { get; }

        /// <summary>最小値を設定し、現在値を補正</summary>
        public abstract void SetMin(T min);

        /// <summary>最大値を設定し、現在値を補正</summary>
        public abstract void SetMax(T max);

        /// <summary>指定値を Min〜Max に制限して設定</summary>
        public abstract void SetClampValue(T value);

        /// <summary>現在値を加算（Maxを超えない）</summary>
        public abstract void Add(T value);

        /// <summary>現在値を減算（Min未満にならない）</summary>
        public abstract void Sub(T value);

        /// <summary>現在値を最大値にする</summary>
        public abstract void SetFull();

        /// <summary>現在値を最小値にする</summary>
        public abstract void SetEmpty();

        protected virtual void Awake()
        {
            _addSubject.AddTo(this);
            _subSubject.AddTo(this);
        }
    }
}