namespace MyUtils.DOTweenUtils
{
    /// <summary>
    /// SetLink で紐づけたGameObjectのライフサイクルに応じたTweenの挙動。
    /// </summary>
    public enum LinkBehaviour
    {
        None,
        KillOnDestroy,
        PauseOnDisable,
        PauseOnDisablePlayOnEnable,
        PauseOnDisableRestartOnEnable,
        KillOnDisable,
        RestartOnDisableRestartOnEnable
    }
}
