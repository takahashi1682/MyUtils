namespace MyUtils.TweenUtils
{
    /// <summary>
    /// ループ再生時の挙動。Incremental は Restart と同様に扱う（値の積み上げは非対応）。
    /// </summary>
    public enum LoopType
    {
        Restart,
        Yoyo,
        Incremental
    }
}
