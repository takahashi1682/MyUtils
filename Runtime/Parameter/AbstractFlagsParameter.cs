namespace MyUtils.Parameter
{
    public abstract class AbstractFlagsParameter<T> : AbstractFlagsParameterBase where T : System.Enum
    {
        public override System.Type FlagEnumType => typeof(T);

        /// <summary>フラグを設定する</summary>
        public void SetFlag(T index, bool value) => SetFlag((int)(object)index, value);

        /// <summary>フラグを反転させる</summary>
        public void ToggleFlag(T index) => ToggleFlag((int)(object)index);

        /// <summary>フラグを持っているか確認</summary>
        public bool HasFlag(T index) => HasFlag((int)(object)index);
    }
}