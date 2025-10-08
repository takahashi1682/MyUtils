using System;
using R3;
using TNRD;
using UnityEngine;

namespace MyUtils.Parameter
{
    public class ParameterUpdater : MonoBehaviour
    {
        public bool IsRunning;

        [SerializeField] private SerializableInterface<AbstractParameter> _parameter;
        [SerializeField] private float _addValue;
        [SerializeField] private float _subValue;
        [SerializeField] private float _interval = 1f;

        private void Start()
        {
            Observable.Interval(TimeSpan.FromSeconds(_interval))
                .Where(_ => IsRunning)
                .Subscribe(_ =>
                {
                    if (_addValue >= 0f)
                    {
                        _parameter.Value.Add(_addValue);
                    }

                    if (_subValue >= 0f)
                    {
                        _parameter.Value.Sub(_subValue);
                    }
                })
                .AddTo(this);
        }
    }
}