using R3;
using System;
using UnityEngine;

namespace MyUtils.Parameter
{
    public abstract class PackedInt16Capacity : MonoBehaviour
    {
        [SerializeField] private SerializableReactiveProperty<long> _current = new(0);
        public ReadOnlyReactiveProperty<long> Current => _current;

        private const int BitPerElement = 4;
        private const int MaxIndex = 16; // 64bit / 4bit
        private const long Mask = 0xF; // 1111 (0~15)

        /// <summary>全要素を最大値(15)に設定</summary>
        public void FillMax() => _current.Value = -1L;

        /// <summary>全要素を0にリセット</summary>
        public void ClearAll() => _current.Value = 0L;

        /// <summary>指定インデックスの値を設定 (0-15)</summary>
        public void SetValue(int index, int value)
        {
            if (IsIndexOutOfRange(index)) return;

            long clampedValue = Math.Clamp(value, 0, 15);
            int shift = index * BitPerElement;

            // 指定箇所の4bitをクリアしてから、新しい値を書き込む
            _current.Value = (_current.Value & ~(Mask << shift)) | (clampedValue << shift);
        }

        /// <summary>指定インデックスの値を取得</summary>
        public int GetValue(int index)
        {
            if (IsIndexOutOfRange(index)) return 0;

            int shift = index * BitPerElement;
            return (int)((_current.Value >> shift) & Mask);
        }

        /// <summary>指定インデックスの値に加算（0-15の範囲を維持）</summary>
        public void AddValue(int index, int amount)
        {
            int current = GetValue(index);
            SetValue(index, current + amount);
        }

        /// <summary>全要素をランダムに設定</summary>
        public void SetRandom(float probability = 1.0f)
        {
            for (int i = 0; i < MaxIndex; i++)
            {
                if (UnityEngine.Random.value <= probability)
                {
                    SetValue(i, UnityEngine.Random.Range(0, 16));
                }
            }
        }

        private static bool IsIndexOutOfRange(int index)
        {
            if (index is < 0 or >= MaxIndex)
            {
                Debug.LogError($"[PackedInt16Capacity] Index {index} is out of range (0-{MaxIndex - 1}).");
                return true;
            }

            return false;
        }
    }
}