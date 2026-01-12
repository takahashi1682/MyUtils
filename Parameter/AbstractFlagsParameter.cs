using R3;
using UnityEngine;

namespace MyUtils.Parameter
{
    public abstract class AbstractFlagsParameter : MonoBehaviour
    {
        [SerializeField] private SerializableReactiveProperty<ulong> _current = new(0);
        public ReadOnlyReactiveProperty<ulong> Current => _current;

        /// <summary>現在のフラグ値を設定する</summary>
        public void SetFlags(ulong value) => _current.Value = value;

        /// <summary>全てのフラグを立てる</summary>
        public void SetFullFlags() => _current.Value = ulong.MaxValue;

        /// <summary>全てのフラグを下ろす</summary>
        public void ClearAllFlags() => _current.Value = 0UL;

        /// <summary>フラグを設定する</summary>
        public void SetFlag(int index, bool value)
        {
            if (IsIndexOutOfRange(index)) return;
            _current.Value = value
                ? _current.Value | (1UL << index)
                : _current.Value & ~(1UL << index);
        }

        /// <summary>フラグを反転させる</summary>
        public void ToggleFlag(int index)
        {
            if (IsIndexOutOfRange(index)) return;
            _current.Value ^= (1UL << index);
        }

        /// <summary>フラグを持っているか確認</summary>
        public bool HasFlag(int index)
        {
            if (IsIndexOutOfRange(index)) return false;
            return (_current.Value & (1UL << index)) != 0;
        }

        public void SetFullRandom(float rate)
        {
            ulong result = 0UL;
            for (int i = 0; i < 64; i++)
            {
                if (Random.value < rate)
                {
                    result |= (1UL << i);
                }
            }

            _current.Value = result;
        }

        private static bool IsIndexOutOfRange(int index)
        {
            if (index is < 0 or >= 64)
            {
                Debug.LogError($"Flag index {index} is out of range (0-63).");
                return true;
            }

            return false;
        }
    }
}