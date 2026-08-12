using System;
using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public interface ILongParameter : IParameter<long>
    {
    }

    /// <summary>
    /// long型のパラメータ。int/floatの範囲を超える通貨・スコアなど大きな数値を扱う場合に使用する。
    /// </summary>
    public abstract class AbstractLongParameter : AbstractNumericParameter<long>, ILongParameter
    {
        [SerializeField] private SerializableReactiveProperty<long> _current = new(1000L);
        public override ReadOnlyReactiveProperty<long> Current => _current;

        [SerializeField] private SerializableReactiveProperty<long> _min = new(0L);
        public override ReadOnlyReactiveProperty<long> Min => _min;

        [SerializeField] private SerializableReactiveProperty<long> _max = new(1000L);
        public override ReadOnlyReactiveProperty<long> Max => _max;

        protected override long Plus(long a, long b) => a + b;
        protected override long Minus(long a, long b) => a - b;
        protected override long Clamp(long value, long min, long max) => Math.Clamp(value, min, max);

        protected override float ToRate(long current, long min, long max) =>
            max == min ? 0f : Mathf.Clamp01((float)(current - min) / (max - min));

        // int範囲を超える値はint.MinValue/MaxValueに丸める（オーバーフローによる符号反転を防ぐ）
        protected override int ToInt(long value) => (int)Math.Clamp(value, int.MinValue, int.MaxValue);
        protected override float ToFloat(long value) => value;

        protected override void SetCurrentValue(long value) => _current.Value = value;
        protected override void SetMinValue(long value) => _min.Value = value;
        protected override void SetMaxValue(long value) => _max.Value = value;

        protected override void Awake()
        {
            base.Awake();
            _current.AddTo(this);
            _min.AddTo(this);
            _max.AddTo(this);
        }
    }
}
