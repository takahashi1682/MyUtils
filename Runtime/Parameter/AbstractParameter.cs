using System;
using MyUtils.UIBinder;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface IParameter<T>
    {
        ReadOnlyReactiveProperty<T> Current { get; }
        ReadOnlyReactiveProperty<T> Min { get; }
        ReadOnlyReactiveProperty<T> Max { get; }

        ReadOnlyReactiveProperty<float> CurrentRate { get; }

        ReadOnlyReactiveProperty<int> CurrentInt { get; }
        ReadOnlyReactiveProperty<float> CurrentFloat { get; }
        ReadOnlyReactiveProperty<string> CurrentString { get; }

        ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; }
        ReadOnlyReactiveProperty<bool> IsAboveHalf { get; }

        Observable<T> OnAdd { get; }
        Observable<T> OnSub { get; }

        ReadOnlyReactiveProperty<bool> IsFull { get; }
        ReadOnlyReactiveProperty<bool> IsEmpty { get; }

        void SetMin(T min);
        void SetMax(T max);
        void SetClampValue(T value);
        void Add(T value);
        void Sub(T value);
        void SetFull();
        void SetEmpty();
    }

    /// <summary>
    /// 汎用的なパラメータクラス（HP, Stamina, Gauge など）。
    /// 使用されないReactivePropertyは遅延初期化されます。
    /// </summary>
    [Serializable]
    public abstract class AbstractParameter<T> : MonoBehaviour,
        IParameter<T>,
        IRateBinder,
        IViewSwitchBinder,
        IValueBinder<int>,
        IValueBinder<float>,
        IValueBinder<string>
    {
        public abstract ReadOnlyReactiveProperty<T> Current { get; }
        public abstract ReadOnlyReactiveProperty<T> Min { get; }
        public abstract ReadOnlyReactiveProperty<T> Max { get; }

        public abstract ReadOnlyReactiveProperty<float> CurrentRate { get; }

        public abstract ReadOnlyReactiveProperty<int> CurrentInt { get; }
        public abstract ReadOnlyReactiveProperty<float> CurrentFloat { get; }
        public abstract ReadOnlyReactiveProperty<string> CurrentString { get; }

        public abstract ReadOnlyReactiveProperty<bool> IsHalfOrLess { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsAboveHalf { get; }

        protected readonly Subject<T> _addSubject = new();
        public Observable<T> OnAdd => _addSubject;

        protected readonly Subject<T> _subSubject = new();
        public Observable<T> OnSub => _subSubject;

        public abstract ReadOnlyReactiveProperty<bool> IsFull { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsEmpty { get; }

        ReadOnlyReactiveProperty<int> IValueBinder<int>.CurrentValue => CurrentInt;
        ReadOnlyReactiveProperty<float> IValueBinder<float>.CurrentValue => CurrentFloat;
        ReadOnlyReactiveProperty<string> IValueBinder<string>.CurrentValue => CurrentString;

        /// <summary>最小値を設定し、現在値を補正</summary>
        public abstract void SetMin(T min);

        /// <summary>最大値を設定し、現在値を補正</summary>
        public abstract void SetMax(T max);

        private void Refresh() => SetClampValue(Current.CurrentValue);

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