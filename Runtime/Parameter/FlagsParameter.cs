using System;

namespace MyUtils.Parameter
{
    /// <summary>
    /// enum Tの各値をビット位置として扱う、型付きのFlagsParameterBase。
    /// </summary>
    [Serializable]
    public class FlagsParameter<T> : FlagsParameterBase where T : Enum
    {
        public override Type FlagEnumType => typeof(T);

        /// <summary>フラグを設定する</summary>
        public void SetFlag(T index, bool value) => SetFlag((int)(object)index, value);

        /// <summary>フラグを反転させる</summary>
        public void ToggleFlag(T index) => ToggleFlag((int)(object)index);

        /// <summary>フラグを持っているか確認</summary>
        public bool HasFlag(T index) => HasFlag((int)(object)index);
    }
}
