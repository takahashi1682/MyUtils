using System;
using R3;
using TNRD;
using UnityEngine;

namespace MyUtils.Parameter
{
    /// <summary>
    /// 一定間隔でAbstractParameterの現在値を増減させる機能（HPの自然回復、毒によるダメージなど）。
    /// </summary>
    public abstract class AbstractParameterUpdater<T> : MonoBehaviour where T : IComparable<T>
    {
        [field: SerializeField] public bool IsRunning { get; set; }

        [SerializeField] protected SerializableInterface<AbstractParameter<T>> _parameter;
        [SerializeField] protected T _addValue;
        [SerializeField] protected T _subValue;
        [SerializeField] protected float _interval = 1f;

        private void Start()
        {
            Observable.Interval(TimeSpan.FromSeconds(_interval))
                .Where(_ => IsRunning)
                .Subscribe(_ =>
                {
                    if (_addValue.CompareTo(default) >= 0)
                    {
                        _parameter.Value.Add(_addValue);
                    }

                    if (_subValue.CompareTo(default) >= 0)
                    {
                        _parameter.Value.Sub(_subValue);
                    }
                })
                .AddTo(this);
        }
    }
}